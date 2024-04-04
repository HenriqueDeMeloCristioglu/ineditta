/**
 *
 * @param {NodeList} rows
 * @param {HTMLElement} button
 * @param {string} fileName //without extension
 */
function exportCsv(rows, button, fileName) {
    const tableRows = Array.from(rows);

    const csvString = tableRows
        .map(row => Array.from(row.cells)
            .map(cell => cell.textContent)
            .join(",")
        ).join("\n")

    console.log(csvString);

    button.setAttribute('href', `data:text/csvcharset=utf-8,${encodeURIComponent(csvString)}`)
    button.setAttribute('download', fileName + '.csv')
}