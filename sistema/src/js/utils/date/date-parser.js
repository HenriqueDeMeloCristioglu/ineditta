class DateParser {
  static numberMonthDictionary = {
    1: 'JAN',
    2: 'FEV',
    3: 'MAR',
    4: 'ABR',
    5: 'MAI',
    6: 'JUN',
    7: 'JUL',
    8: 'AGO',
    9: 'SET',
    10: 'OUT',
    11: 'NOV',
    12: 'DEZ'
  };

  static monthNumberDictionary = {
    'JAN': 1,
    'FEV': 2,
    'MAR': 3,
    'ABR': 4,
    'MAI': 5,
    'JUN': 6,
    'JUL': 7,
    'AGO': 8,
    'SET': 9,
    'OUT': 10,
    'NOV': 11,
    'DEZ': 12
};

  static fromString(dateString) {
    var parts = dateString.split("-");
    var year = parseInt(parts[0]);
    var month = parseInt(parts[1]) - 1;
    var day = parseInt(parts[2]);
    
    return new Date(year, month, day);
  }

  static toString(date) {
      var year = date.getFullYear();
      var month = String(date.getMonth() + 1).padStart(2, '0');
      var day = String(date.getDate()).padStart(2, '0');
      
      return year + '-' + month + '-' + day;
  }

  static toDataBaseString(date) {
    var year = date.getFullYear();
    var month = String(date.getMonth() + 1)

    return DateParser.numberMonthDictionary[month] + "/" + year;
  }

  static fromDataBaseString(dataBaseString) {
    var splitData = dataBaseString.split("/");
    var month = splitData[0]?.toUpperCase();
    var year = Number(splitData[1]);

    return new Date(year, DateParser.monthNumberDictionary[month] - 1, 1);
  }
}

export default DateParser;