# Google-Drive-Asset-Sync

**Not totally completed yet**
Instructions to get all set up:
- Go to the [Google Api Console](https://console.cloud.google.com/apis/dashboard?project=academic-atlas-230017)
- Create a new project and select the Google Drive Api
- Create a new Service Account Key Credential
- Save the key and then rename it to "key.p12" and put it in the same directory as the program
- Copy this template to a file in the directory and name it "config.ini":

```
Server_Account = **The email that is associated with the credential you made**
Current_Project = **Not Necessary to fill in at this time, though put your project name here**
Current_Project_ID = 
Current_Project_Models_ID = 
Current_Project_Audio_ID = 
Current_Project_Music_ID = 
Auto_Launch = true
Auto_Launch_Unity = true
Unity_Directory = **Path Unity Hub or Unity for the program to auto launch Unity**
```
- To actually fill out the "ID", you will need to open the program and use the "ls" command to get the file IDs and their parents. **I will mention at the moment, it is not very intuitive to use**
- Once you find the folder name and the correct parent project, put the ids into the correct spots. The Folders can be named anything you want, doesn't matter to the program.
- After this, you're all set to use the program.
