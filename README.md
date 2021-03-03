# Employees
ASP.NET MVC Project with CRUD Operation
**Task – Integration with external party**
You are given a csv file – this is the input from external system
Create a SQL Server database with only one table – Employees. That table should have as many columns as needed to accommodate data from the file.
Create a simple ASP.Net MVC web site with one page. That page should contain 
Browse File control
a button to execute the import 
a grid/table (described below).
When user selects the file and clicks on Import button the program should parse the file, get data and insert to the database. The page should report on how many rows were successfully processed.
The added rows should be shown in the grid on the same page. Data should be sorted by surname ascending. Grid should support sorting, searching and edit functionality.
It is advised to use a third-party library for the grid.
Comment and unit-test the code
