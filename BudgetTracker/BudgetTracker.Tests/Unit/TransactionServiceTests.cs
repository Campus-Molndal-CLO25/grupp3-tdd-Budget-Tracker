using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services;
using FluentAssertions;
using NSubstitute;

namespace BudgetTracker.Tests.Unit;

public class TransactionServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsNonPositiveAmount()
    {
        var transactions = Substitute.For<ITransactionRepository>();
        var accounts = Substitute.For<IAccountRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        var service = new TransactionService(transactions, accounts, categories);

        var act = () => service.CreateAsync(new CreateTransactionDto
        {
            AccountId = 1,
            CategoryId = 2,
            Amount = 0,
            Type = TransactionType.Expense,
            Date = DateTime.UtcNow
        });

        var ex = await Assert.ThrowsAsync<ArgumentException>(act);
        ex.Message.Should().Be("Amount must be greater than zero");
    }

    [Fact]
    public async Task CreateAsync_UpdatesAccountBalance()
    {
        var transactions = Substitute.For<ITransactionRepository>();
        var accounts = Substitute.For<IAccountRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        var account = new Account { Id = 1, Name = "Main", CurrentBalance = 100 };
        var category = new Category { Id = 2, Name = "Salary" };
        accounts.GetByIdAsync(1).Returns(account);
        categories.GetByIdAsync(2).Returns(category);
        transactions.AddAsync(Arg.Any<Transaction>()).Returns(call => call.Arg<Transaction>());
        var service = new TransactionService(transactions, accounts, categories);

        var result = await service.CreateAsync(new CreateTransactionDto
        {
            AccountId = 1,
            CategoryId = 2,
            Amount = 50,
            Type = TransactionType.Income,
            Date = new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc),
            Description = "Pay"
        });

        account.CurrentBalance.Should().Be(150);
        result.AccountName.Should().Be("Main");
        result.CategoryName.Should().Be("Salary");
        await accounts.Received(1).UpdateAsync(account);
        await accounts.Received(1).SaveChangesAsync();
        await transactions.Received(1).AddAsync(Arg.Any<Transaction>());
    }

    [Fact]
    public async Task UpdateAsync_AdjustsBalanceOnSameAccount()
    {
        var transactions = Substitute.For<ITransactionRepository>();
        var accounts = Substitute.For<IAccountRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        var transaction = new Transaction
        {
            Id = 5,
            AccountId = 1,
            CategoryId = 2,
            Amount = 20,
            TransactionType = TransactionType.Expense
        };
        var account = new Account { Id = 1, Name = "Main", CurrentBalance = 100 };
        var category = new Category { Id = 3, Name = "Bonus" };
        transactions.GetByIdAsync(5).Returns(transaction);
        accounts.GetByIdAsync(1).Returns(account);
        categories.GetByIdAsync(3).Returns(category);
        var service = new TransactionService(transactions, accounts, categories);

        var result = await service.UpdateAsync(5, new UpdateTransactionDto
        {
            AccountId = 1,
            CategoryId = 3,
            Amount = 40,
            Type = TransactionType.Income,
            Date = new DateTime(2025, 2, 12, 0, 0, 0, DateTimeKind.Utc),
            Description = "Update"
        });

        account.CurrentBalance.Should().Be(160);
        result!.CategoryName.Should().Be("Bonus");
        await accounts.Received(1).UpdateAsync(account);
        await accounts.Received(1).SaveChangesAsync();
        await transactions.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_AdjustsBalanceOnAccountChange()
    {
        var transactions = Substitute.For<ITransactionRepository>();
        var accounts = Substitute.For<IAccountRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        var transaction = new Transaction
        {
            Id = 7,
            AccountId = 1,
            CategoryId = 2,
            Amount = 30,
            TransactionType = TransactionType.Expense
        };
        var oldAccount = new Account { Id = 1, Name = "Old", CurrentBalance = 100 };
        var newAccount = new Account { Id = 2, Name = "New", CurrentBalance = 200 };
        var category = new Category { Id = 2, Name = "Transfer" };
        transactions.GetByIdAsync(7).Returns(transaction);
        accounts.GetByIdAsync(1).Returns(oldAccount);
        accounts.GetByIdAsync(2).Returns(newAccount);
        categories.GetByIdAsync(2).Returns(category);
        var service = new TransactionService(transactions, accounts, categories);

        var result = await service.UpdateAsync(7, new UpdateTransactionDto
        {
            AccountId = 2,
            CategoryId = 2,
            Amount = 10,
            Type = TransactionType.Income,
            Date = new DateTime(2025, 2, 12, 0, 0, 0, DateTimeKind.Utc)
        });

        oldAccount.CurrentBalance.Should().Be(130);
        newAccount.CurrentBalance.Should().Be(210);
        result!.AccountName.Should().Be("New");
        await accounts.Received(1).UpdateAsync(oldAccount);
        await accounts.Received(1).UpdateAsync(newAccount);
    }
}
