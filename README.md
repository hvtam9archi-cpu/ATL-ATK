# ATL-ATK Plugin for AutoCAD

## Overview
The **ATL-ATK** plugin is a powerful AutoCAD Add-in built with C# .NET to automate the extraction, counting, and tabulation of Block Attributes and Dynamic Parameters. It simplifies the process of generating detailed statistics and quantity takeoffs directly within your AutoCAD drawings.

This project was originally inspired by traditional AutoLISP workflows but has been fully rewritten in C# .NET to provide a modern, robust, and highly performant experience with a standardized User Interface.

## Key Features

### 1. Main Configuration & Tabulation
*   **`ATL`**: Open the global settings panel. Configure table styles, text styles, column definitions (supporting expressions and formulas), and block filtering options.
*   **`ATK`**: The core command for detailed block attribute statistics. Opens an interactive dialog to select options, sort data, and place a beautifully formatted summary table in your drawing. Features include automatic sequence numbering (STT), sum rows, and dynamic column widths.

### 2. Fast Counting Commands
*   **`ATC`**: Count the number of blocks grouped by specific Attribute tags.
*   **`DYC`**: Count the number of blocks grouped by Dynamic Parameter values.
*   **`ATDYC`**: A combination of ATC and DYC, counting blocks based on both Attribute and Dynamic Parameter values.
*   **`BLC`**: Count the total number of blocks, with an optional mode to group counts by Visibility States.
*   **`NDC`**: Count specific text contents, analyzing both raw Text objects and Block Attribute values.

### 3. Quick Statistics Commands
*   **`AT1`**: Extract values of a single specific Attribute tag.
*   **`DY1`**: Extract values of a single specific Dynamic Parameter.
*   **`ATDY1`**: Extract values for both Attribute tags and Dynamic Parameters.
*   **`ATC`, `DYC`, `ATDYC`**: (Quick counting equivalents).
*   **`ATKHELP`**: Show help and documentation.

## Architecture & Code Structure

The solution follows clean architectural principles for maintainability:
*   **`Commands/`**: AutoCAD entry points (CommandMethods) wrapping execution logic with global try-catch blocks to prevent Host crashes.
*   **`Models/`**: Plain C# objects representing Blocks, Attributes, Dynamic Properties, and Settings.
*   **`Services/`**: Business logic cleanly separated from UI and Command handling. Includes:
    *   `BlockQueryService`: Safely reads from AutoCAD Database using efficient Transactions.
    *   `TableGeneratorService`: Generates AutoCAD `Table` entities dynamically.
    *   `SortingService`: Organizes extracted data by positions or values.
    *   `TagParserService`: Parses custom tag expressions (e.g., calculations or concatenations of attributes).
*   **`UI/`**: WPF Windows and Dialogs utilizing a centralized `ThemeManager` for a consistent Dark Mode experience.
*   **`Helpers/`**: Utilities for text formatting, math, and extension methods.

## Development Principles (Followed strictly)
1.  **KISS & YAGNI**: Features are implemented practically without over-engineering.
2.  **Resource Management**: All AutoCAD DBObjects, Transactions, and unmanaged resources are strictly disposed of via `using` statements to prevent memory leaks.
3.  **UI Performance**: Heavy data extraction logic runs without blocking the main UI unnecessarily.
4.  **Aesthetics**: The UI components utilize modern Dark Mode styling for a professional aesthetic.

## Installation & Deployment

1.  Compile the project using Visual Studio (ensure correct AutoCAD ObjectARX references are configured for your target version).
2.  The build output will generate `ATL-ATK.dll`.
3.  Load the DLL in AutoCAD using the `NETLOAD` command.
4.  Alternatively, the project includes a `PackageContents.xml` for AutoCAD App Store bundle deployment.

## System Requirements
*   **AutoCAD version**: AutoCAD 2021 to 2024 (SeriesMin="R24.0", SeriesMax="R24.3")
*   **Framework**: .NET Framework 4.8
*   **OS**: Windows 64-bit
