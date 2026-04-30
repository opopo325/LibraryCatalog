const API_BASE = "/api";

// Helper to render an array of objects into the table
function renderTable(data) {
    const tbody = document.getElementById('resultsTable');
    tbody.innerHTML = ''; // Clear current table

    if (!data || data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" class="p-4 text-center text-gray-500">No data found in database.</td></tr>';
        return;
    }

    data.forEach(item => {
        // Since we use TPH, we can check for specific properties to determine the type
        const isBook = item.author !== undefined; 
        const typeLabel = isBook ? '📚 Book' : '📰 Magazine';
        const extraInfo = isBook ? `Author: ${item.author}` : `Issues/Year: ${item.issuesPerYear}`;

        const tr = document.createElement('tr');
        tr.className = "border-b hover:bg-gray-50";
        tr.innerHTML = `
            <td class="p-4">${typeLabel}</td>
            <td class="p-4 font-semibold">${item.title}</td>
            <td class="p-4">${item.theme}</td>
            <td class="p-4">${extraInfo}</td>
            <td class="p-4">
                <button onclick="deleteItem(${item.id})" class="text-red-500 hover:text-red-700 font-bold">Delete</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

// 1. Fetch ALL items
async function loadAllItems() {
    try {
        const response = await fetch(`${API_BASE}/items`);
        const data = await response.json();
        renderTable(data);
    } catch (error) {
        console.error('Error fetching items:', error);
    }
}

// 2. Fetch Computer Magazines
async function loadComputerMagazines() {
    try {
        const response = await fetch(`${API_BASE}/magazines/computers`);
        const data = await response.json();
        renderTable(data);
    } catch (error) {
        console.error('Error:', error);
    }
}

// 3. Fetch ONLY magazines
async function loadOnlyMagazines() {
    try {
        const response = await fetch(`${API_BASE}/magazines`);
        const data = await response.json();
        renderTable(data);
    } catch (error) {
        console.error('Error:', error);
    }
}

// 4. Fetch sorted book titles (Special case: returns array of strings, not objects)
async function loadSortedBookTitles() {
    try {
        const response = await fetch(`${API_BASE}/books/sorted-titles`);
        const titles = await response.json();
        
        const tbody = document.getElementById('resultsTable');
        tbody.innerHTML = ''; 

        titles.forEach(title => {
            const tr = document.createElement('tr');
            tr.className = "border-b hover:bg-gray-50";
            tr.innerHTML = `
                <td class="p-4">📚 Book</td>
                <td class="p-4 font-semibold text-green-600">${title}</td>
                <td class="p-4 text-gray-400">-</td>
                <td class="p-4 text-gray-400">-</td>
                <td class="p-4 text-gray-400">-</td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error('Error:', error);
    }
}

// 5. Delete item by ID
async function deleteItem(id) {
    if(confirm("Are you sure you want to delete this item?")) {
        try {
            await fetch(`${API_BASE}/items/${id}`, { method: 'DELETE' });
            loadAllItems(); // Refresh the table
        } catch (error) {
            console.error('Error deleting item:', error);
        }
    }
}

document.getElementById('addBookForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const newBook = {
        title: document.getElementById('bTitle').value,
        theme: document.getElementById('bTheme').value,
        author: document.getElementById('bAuthor').value,
        publisher: document.getElementById('bPublisher').value,
        year: parseInt(document.getElementById('bYear').value),
        language: document.getElementById('bLanguage').value,
        pagesCount: parseInt(document.getElementById('bPages').value),
        price: parseFloat(document.getElementById('bPrice').value)
    };

    try {
        const response = await fetch(`${API_BASE}/books`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newBook)
        });

        if (response.ok) {
            this.reset();
            loadAllItems(); 
        }
    } catch (error) {
        console.error('Error adding book:', error);
    }
});

document.getElementById('addMagazineForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const newMagazine = {
        title: document.getElementById('mTitle').value,
        theme: document.getElementById('mTheme').value,
        year: parseInt(document.getElementById('mYear').value),
        language: document.getElementById('mLanguage').value,
        pagesCount: parseInt(document.getElementById('mPages').value),
        issueNumber: parseInt(document.getElementById('mIssueNum').value),
        issuesPerYear: parseInt(document.getElementById('mIssuesPerYear').value),
        annualSubscriptionPrice: parseFloat(document.getElementById('mPrice').value)
    };

    try {
        const response = await fetch(`${API_BASE}/magazines`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newMagazine)
        });

        if (response.ok) {
            this.reset();
            loadAllItems();
        }
    } catch (error) {
        console.error('Error adding magazine:', error);
    }
});

// Load all items when the page first opens
window.onload = loadAllItems;