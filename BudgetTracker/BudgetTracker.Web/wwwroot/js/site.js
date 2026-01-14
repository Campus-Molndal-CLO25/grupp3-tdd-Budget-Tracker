const formatCurrency = (value) => {
  const number = Number.isFinite(value) ? value : 0;
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD"
  }).format(number);
};

const parseBalance = (row) => {
  const raw = row?.dataset?.balance;
  if (raw !== undefined) {
    const parsed = Number.parseFloat(raw);
    return Number.isFinite(parsed) ? parsed : 0;
  }

  const text = row?.textContent ?? "";
  const cleaned = text.replace(/[^0-9.-]/g, "");
  const parsed = Number.parseFloat(cleaned);
  return Number.isFinite(parsed) ? parsed : 0;
};

const updateSummary = (rows) => {
  const total = rows.reduce((sum, row) => {
    const balance = parseBalance(row);
    return sum + balance;
  }, 0);

  const totalBalance = document.querySelector("#totalBalance");
  const totalAccounts = document.querySelector("#totalAccounts");
  if (totalBalance) totalBalance.textContent = formatCurrency(total);
  if (totalAccounts) totalAccounts.textContent = rows.length.toString();
};

const getRows = () => Array.from(document.querySelectorAll("#accountsTable tbody tr"));

const applyFilter = () => {
  const query = document.querySelector("#accountSearch")?.value?.toLowerCase() ?? "";
  const rows = getRows();
  const visibleRows = rows.filter((row) => {
    const name = row.dataset.name?.toLowerCase() ?? "";
    const type = row.dataset.type?.toLowerCase() ?? "";
    return name.includes(query) || type.includes(query);
  });

  rows.forEach((row) => {
    row.style.display = visibleRows.includes(row) ? "" : "none";
  });

  const emptyState = document.querySelector("#emptyState");
  if (emptyState) {
    emptyState.classList.toggle("active", visibleRows.length === 0);
  }

  updateSummary(visibleRows);
};

const applySort = () => {
  const tableBody = document.querySelector("#accountsTable tbody");
  const sortValue = document.querySelector("#accountSort")?.value ?? "name-asc";
  if (!tableBody) return;

  const rows = getRows();
  const sorted = rows.slice().sort((a, b) => {
    const nameA = a.dataset.name?.toLowerCase() ?? "";
    const nameB = b.dataset.name?.toLowerCase() ?? "";
    const balanceA = Number.parseFloat(a.dataset.balance ?? "0");
    const balanceB = Number.parseFloat(b.dataset.balance ?? "0");

    switch (sortValue) {
      case "name-desc":
        return nameB.localeCompare(nameA);
      case "balance-asc":
        return balanceA - balanceB;
      case "balance-desc":
        return balanceB - balanceA;
      default:
        return nameA.localeCompare(nameB);
    }
  });

  sorted.forEach((row) => tableBody.appendChild(row));
  applyFilter();
};

const bindFormValidation = () => {
  const form = document.querySelector(".account-form");
  if (!form) return;

  const nameInput = form.querySelector("input[name='Name']");
  const balanceInput = form.querySelector("input[name='InitialBalance']");
  const submitButton = form.querySelector("button[type='submit']");

  const updateState = () => {
    const nameValid = (nameInput?.value ?? "").trim().length > 0;
    const balanceValue = Number.parseFloat(balanceInput?.value ?? "0");
    const balanceValid = Number.isFinite(balanceValue) && balanceValue >= 0;
    if (submitButton) submitButton.disabled = !(nameValid && balanceValid);
  };

  nameInput?.addEventListener("input", updateState);
  balanceInput?.addEventListener("input", updateState);
  updateState();
};

document.addEventListener("DOMContentLoaded", () => {
  applySort();
  applyFilter();
  bindFormValidation();

  document.querySelector("#accountSearch")?.addEventListener("input", applyFilter);
  document.querySelector("#accountSort")?.addEventListener("change", applySort);
});
