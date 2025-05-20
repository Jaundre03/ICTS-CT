using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using ICTS_CT.Models;

namespace ICTS_CT.Services
{
    public class ExcelService
    {
        public List<Member> ImportMembersFromExcel(string filePath)
        {
            var members = new List<Member>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException("The Excel file was not found.", filePath);

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet?.Dimension == null)
                throw new InvalidDataException("The Excel sheet is empty or improperly formatted.");

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                string id = worksheet.Cells[row, 1].Text;
                string lastName = worksheet.Cells[row, 2].Text;
                string firstName = worksheet.Cells[row, 3].Text;
                string middleName = worksheet.Cells[row, 4].Text;

                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(lastName))
                    continue;

                string fullName = $"{lastName}, {firstName} {middleName}".Trim();

                members.Add(new Member
                {
                    ID = id,
                    DisplayName = fullName,
                    IsChecked = false
                });
            }

            return members;
        }
    }
}
