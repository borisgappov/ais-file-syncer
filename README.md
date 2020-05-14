## Ais File Syncer test application

[![Countries test app](https://img.youtube.com/vi/x99y4ok2Cx0/0.jpg)](https://www.youtube.com/watch?v=x99y4ok2Cx0)

### Task description

You will have access to a library which provides an API to list the user’s available files. There’s a AisUriProviderApi.AisUriProvider class and it has the public IEnumerable<Uri> Get(); method, which returns your file’s URIs. Let’s assume that you should always have 10 files at a time on your storage, but those might be totally different ones.
 
You can download this class library from:
https://bitstore.blob.core.windows.net:443/test/AisUriProviderApi.zip
 
Write an application using a C# based technology, which accomplishes the following :
 
- Lists the currently available 10 files
- Refresh the list automatically every 5 minutes, or when the user’s requesting it. We can call this the sync operation.
- Store the last 10 files locally and load it on app start-up.
- Delete any unnecessary files after syncing.
- Download the files in a parallel way, where the user should be able to specify the degree of parallelism. For example you shouldn’t run more than N tasks at a time and there should not be download tasks explicitly waiting for each other.
- The user should be able to cancel sync operations
- Display a dialog to the user if they want to exit while files are being downloaded. The user should be able to quit anyway.

- The application should be able to represent the files visually, preferably in grid
- If the file is an image, the application should display the image
- If the file is text based, you should show the preview of the text
- For anything else you should show a placeholder

- Errors should be handled gracefully, and displayed to the user
 
We would like to ask you to pay attention to code quality. 
Write a clean code, which is easy to test (and test it) and the concerns are well separated from each other.
We’re interested in how you would architect a solution from the ground up. 

For any details not covered by the requirements or that are vague it is up to you to deal with it.
Please leave your comments, assumptions and decisions in a separate text file.