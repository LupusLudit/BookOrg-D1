# BookOrg

> **Note:** This is a school project.
*Solution to the D1 assignment*

BookOrg is a library management application written in C#, using the WPF (Windows Presentation Foundation) framework and .NET 8.0.
It provides an interface for managing library resources like books, authors, genres, customers, and loan transactions.
The system uses a Microsoft SQL Server backend, utilizing a Data Access Object (DAO) pattern for clean separation of concerns.

## Project Structure

```
BookOrg/
├── Docs/
├── Sql/
│   └── BookOrg.sql
├── Src/
│   ├── Debugging/
│   ├── Logic/
│   │   ├── Connection/
│   │   ├── Core/
│   │   └── Importing/
│   ├── Safety/
│   └── UI/
│       ├── ConnectionWindow/
│       └── DBInteraction/
├── TestCases/
└── App.config
```

* Docs/ – XML Documentation files for classes and members.
* Sql/ – SQL scripts for database schema and test data.
* Src/ – Source code directory
	* Debugging/ – Debugging utilities and logging functionality.
	* Logic/ – Core application logic.
		* Connection/ – Database connection factory and validation logic.
		* Core/ - DAO implementations and DB Entity definitions.
		* Importing/ - CSV data import logic.
	* Safety/ - Standardized error handling and safe execution.
	* UI/ – Contains all user interface components.
		* ConnectionWindow/ – Initial DB connection startup window
		* DBInteraction/ - Main application windows and entity management controls
* TestCases/ - Detailed PDF test scenarios for quality assurance
* App.config – Application configuration for DB connection

## Setup

### 1. Database Creation

Before running the application, you need to set up the database.
Make sure to delete any tables, views, etc. whose names conflict with those created by the script if you are reusing an existing database.
The application requires a Microsoft SQL Server database.
To set it up:

1. Open SQL Server Management Studio (SSMS).
2. Open the file Sql/BookOrg.sql located in the project repository.
3. Execute the script. This will create the necessary tables and seed them with initial test data. Note: The script is to be executed from top to bottom. First the transaction containing the table definitions, then proceed to the views and finally insert the example data. Each section is to be executed separately (due to SQL server restrictions).

### 2. Application Configuration

Before running the program, you must provide your database credentials in the App.config (BookOrg.dll.config in the release) file:

1. Locate App.config (BookOrg.dll.config in the release) in the root directory.

2. Edit the following keys in the <appSettings> section:
	* DataSource: Your SQL Server instance address (for testers: 193.85.203.188 or a different Microsoft SQL Server data source if you have your own).
	* Database: The name of the database (for testers: your school assigned username or a different one if you have your own db).
	* Login: Your database username (for testers: your school assigned username or a different one if you have your own db).
	* Password: Your database password.

3. Save the changes.

### 3. Running the Program

To run the application:

1. Download the latest release of BookOrg: [v.1.0.0](https://github.com/LupusLudit/BookOrg-D1/releases/tag/v1.0.0)
2. Extract the contents to a folder of your choice.
3. Edit the App.config (BookOrg.dll.config in the release) file in the extracted folder to include your database credentials as described in the Application Configuration section.
4. Locate and run the `BookOrg.exe` file in the extracted folder.

## Using the Program

After editing the App.config (BookOrg.dll.config in the release) file and starting the application, the Connection Window will appear.
If all connection parameters are correct, you will be informed about a successful connection and taken to the Main Menu.
If the connection fails, you will not be able to proceed until the parameters are corrected.

### Navigation

The Main Menu provides access to five management modules: Authors, Genres, Books, Customers, and Loans.
Use the "Back to Menu" button at the top of the window to return to this screen at any time.

### Entity Management

* Listing & Editing: Data is displayed in a grid. You can edit entries directly by clicking on cells and typing.
* Adding: Click the Add (+) button in any module to create a new entry.
* Deleting: Click the Remove (X) button on any row. A confirmation dialog will warn you if deleting the item affects other entities (e.g., deleting an author used in a book).
* Importing: The Authors, Genres, and Customers modules support importing data from CSV files via the "Import from csv" button. The CSV must follow the specified format (see Docs/BookOrgProjectDocumentation.pdf for more information).

### Loan Logic

* Creating a loan automatically decreases the "In Stock" count of the selected book.
* Loans have statuses: active, overdue, or returned.
* Changing a loan status to returned automatically sets the return date to today and increases the book's stock by 1.
* Validation prevents setting a "Due Date" that is earlier than the "Start Date".

### Information for Testers
If you are performing verification for this project, please refer to the files in the TestCases/ directory for step-by-step instructions:

* T_0 & T_1 (AppStartTestCase.pdf): Verify program startup and invalid configuration detection.
* T_2 (CoreLogicTestCase.pdf): Verify database transactions, views, and validation logic during the loan process.
* T_3 (DataImportTestCase.pdf): Verify CSV importing and the management of Many-to-Many relationships between books, authors, and genres.

Ensure the database is freshly created using the provided SQL script before starting the test cases to match the expected initial state.

## Notes
**This project was developed as a school project**  
BookOrg offers only basic features but even those are in an essence usable since it can be used as a template or foundation for further development.
This project can be modified and used as described in the LICENSE: GNU GENERAL PUBLIC LICENSE version 2.