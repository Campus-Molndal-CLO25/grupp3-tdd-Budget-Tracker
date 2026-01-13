using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BudgetTracker.Tests.Unit;

public class AccountServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsBlankName()
    {
        var repo = Substitute.For<IAccountRepository>();
        var service = new AccountService(repo);

        var act = () => service.CreateAsync(new CreateAccountDto { Name = "   " });

        var ex = await Assert.ThrowsAsync<ArgumentException>(act);
        ex.Message.Should().Be("Name is required");
    }

    [Fact]
    public async Task CreateAsync_RejectsNegativeBalance()
    {
        var repo = Substitute.For<IAccountRepository>();
        var service = new AccountService(repo);

        var act = () => service.CreateAsync(new CreateAccountDto
        {
            Name = "Everyday",
            InitialBalance = -10
        });

        var ex = await Assert.ThrowsAsync<ArgumentException>(act);
        ex.Message.Should().Be("Initial balance cannot be negative");
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateName()
    {
        var repo = Substitute.For<IAccountRepository>();
        repo.NameExistsAsync("Cash").Returns(true);
        var service = new AccountService(repo);

        var act = () => service.CreateAsync(new CreateAccountDto
        {
            Name = "Cash",
            AccountType = AccountType.Cash,
            InitialBalance = 20
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        ex.Message.Should().Be("Account name must be unique");
    }

    [Fact]
    public async Task CreateAsync_PersistsTrimmedAccount()
    {
        var repo = Substitute.For<IAccountRepository>();
        repo.NameExistsAsync("Main").Returns(false);
        repo.AddAsync(Arg.Any<Account>()).Returns(call => call.Arg<Account>());
        var service = new AccountService(repo);

        var result = await service.CreateAsync(new CreateAccountDto
        {
            Name = "  Main ",
            AccountType = AccountType.Checking,
            InitialBalance = 250
        });

        result.Name.Should().Be("Main");
        result.CurrentBalance.Should().Be(250);
        await repo.Received(1).AddAsync(Arg.Is<Account>(a =>
            a.Name == "Main" && a.InitialBalance == 250 && a.CurrentBalance == 250));
    }
}
