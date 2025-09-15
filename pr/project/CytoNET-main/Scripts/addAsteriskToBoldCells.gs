function markBoldCellsWithAsterisk() {
  // Get the active spreadsheet and sheet
  var spreadsheet = SpreadsheetApp.getActiveSpreadsheet();
  var sheet = spreadsheet.getActiveSheet();

  // Define column indices (1-based in Google Sheets)
  var startCol = 8; // Column H is index 8
  var endCol = 28; // Column AB is index 28
  var colCount = endCol - startCol + 1; // 21 columns total

  // Find the last row with data in the sheet
  var lastRow = sheet.getLastRow();

  // If the last row is less than 3, there's no data to process below row 2
  if (lastRow < 3) {
    SpreadsheetApp.getUi().alert("No data found below row 2 in the specified columns.");
    return;
  }

  // Set the range to process (H3:AB[lastRow])
  var processRange = sheet.getRange(3, startCol, lastRow - 2, colCount);

  // Get the rich text objects to check for bold formatting
  var richTextValues = processRange.getRichTextValues();
  var values = processRange.getValues();
  var hasChanges = false;

  // Loop through each cell in the range
  for (var i = 0; i < richTextValues.length; i++) {
    for (var j = 0; j < richTextValues[i].length; j++) {
      // Skip empty cells
      if (values[i][j] !== "") {
        var richText = richTextValues[i][j];
        var text = richText.getText();

        // Skip if already ends with asterisk
        if (text.slice(-1) === "*") {
          continue;
        }

        // Check for bold formatting
        var isBold = false;
        var runs = richText.getRuns();

        // Check if any part of the text is bold
        for (var k = 0; k < runs.length; k++) {
          if (runs[k].getTextStyle().isBold()) {
            isBold = true;
            break;
          }
        }

        // If any part is bold, add asterisk
        if (isBold) {
          values[i][j] = text + "*";
          hasChanges = true;
        }
      }
    }
  }

  // Only update if changes were made
  if (hasChanges) {
    processRange.setValues(values);
  }

  // Show completion message
  SpreadsheetApp.getUi().alert(
    "Process complete. Bold cells in columns H to AB (starting from row 3) have been marked with an asterisk (*)"
  );
}
