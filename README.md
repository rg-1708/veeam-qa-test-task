# Folder Synchronization Program
This program is a solution to the test task provided by Veeam for the Internal Development in QA (SDET) team. 
It synchronizes two folders, maintaining a full, identical copy of the source folder at the replica folder.

## Installation
Simply pull the repository, and run the cs file in your favourite IDE.

## command-line arguments example
In my case, these cli arguments were passed, to the command line arguments in Rider IDE.
  ```

  C:\Users\[user]\Desktop\real C:\Users\[user]\Desktop\replica 5000 C:\Users\[user]\Desktop\SyncLog.txt

  ```
replace [user] with your user, or simply select any other folder, the folders will be created if they don't exitst.

## Example output: 
- CLI:
  
  ![image](https://github.com/rg-1708/veeam-qa-test-task/assets/52547857/fbc266d9-0b4b-44d1-a570-2b4b4d92a816)

- Sync File:
  
  ![image](https://github.com/rg-1708/veeam-qa-test-task/assets/52547857/b210823c-bd04-4fd5-836a-fe841c16a34a)
  
