# dotnet-k8s-servicetemplate
Repo for dotnet based microservice templates with multiple projects

This repository contains a multi project template sample that demonstrates a micro-service in C#. 
It also has the required assets for deploying to kubernetes and 1 click run-debug loop with "Project Tye"

## Using the template and building out your own vsix

* Since this is the base template project, make your modifications in this project and generate an "exported.zip" file. (Just package all the files and folders into exported.zip)
* Open the "vsix" template project at https://github.com/pattisapu01/dotnet-k8s-servicetemplate-vsix and add the "exported.zip"
* run the vsix project top open experimental instance of Visual Studio. You can click on "New Project" and select "Cloud Microservice (Mutli Project) .Net 6 Template"
* Once satisfiesd of your changes, you can just install the vsix and start using.

## Running the Samples From the Command Line
* Clone this repository:
```
    $ git clone https://github.com/pattisapu01/dotnet-k8s-servicetemplate.git
```
