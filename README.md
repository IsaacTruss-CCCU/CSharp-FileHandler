## Overview
The `FileHandler` class provides an abstraction for managing and interacting with files in a secure and structured way. It includes functionalities for reading, writing, appending, and managing file properties. It also leverages `FileIOPermission` to check for access permissions.

## Namespace
```csharp
namespace fileHanding
```

## Dependencies
- **System.IO**: For file operations.
- **System.Security.Permissions**: For permission checks.

## Constructor

`FileHandler(string path, string mode, bool createIfNotFound = true)`
Initializes a new instance of the `FileHandler` class.
### Parameters:
- **`path`** (string): The file path to manage.
- **`mode`** (string): The mode in which the file is to be handled.
- **`createIfNotFound`** (bool): Whether to create the file if it doesn't exist. Defaults to `true`.

## Private Properties
- **`logPath`** (string): Reserved for logging purposes.
- **`path`** (string): The file path being managed.
- **`mode`** (string): The operation mode of the file.
- **`interactor`** (`FileInteractor`): Handles file-specific operations.
- **`createIfNotFound`** (bool): Determines if the file should be created when missing.
- **`properties`** (`Dictionary<string, string>`): Stores file metadata.
- **`fileInfo`** (`FileInfo`): Stores information about the file.

## Public Methods

### 1. **File Information**
#### `Dictionary<string, string> getFileInfo()`
Returns the stored file properties such as size, creation time, last access time, and last edit time.

### 2. **Permission Checks**
#### `bool isReadable()`
Checks if the file is readable.

#### `bool isWriteable()`
Checks if the file is writable.

#### `bool isAppendable()`
Checks if the file can be appended to.

**Note:** Each method uses `FileIOPermission` to verify access.

### 3. **File Content Operations**
#### `void append(List<string> lines, bool writeLines = true)`
Appends a list of lines to the file. Optionally allows toggling line-by-line writing.

#### `void write(List<string> lines, bool writeLines = true)`
Overwrites the file with the provided list of lines. Optionally allows toggling line-by-line writing.

#### `string[] read()`
Reads the file and returns its content as an array of strings.

### 4. **File Management**
#### `void createFile()`
Creates the file if it doesn't exist.

#### `void deleteFile()`
Deletes the file.

#### `void moveFile(string newPath)`
Moves the file to a new location and updates the file path.

#### `void copyFile(string newPath)`
Creates a copy of the file at the specified location.

## Private Methods
### `void updateInfo()`
Takes the metadata of the file (size, creation time, last access time, etc.) and stores it in the `properties` dictionary.

## Notes
1. **`FileIOPermission`**: Ensures secure file access by demanding appropriate permissions for read, write, and append operations.
2. **Error Handling**: Exception handling should be added when deploying in production environments to handle unauthorized access or file-related errors gracefully.
