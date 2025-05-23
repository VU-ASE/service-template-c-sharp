<h1 align="center">service template for <code>C#</code></h1>
<div align="center">
  <a href="https://github.com/VU-ASE/service-template-c-sharp/releases/latest">Latest release</a>
  <span>&nbsp;&nbsp;•&nbsp;&nbsp;</span>
  <a href="https://ase.vu.nl/docs/framework/glossary/service">About a service</a>
  <span>&nbsp;&nbsp;•&nbsp;&nbsp;</span>
  <a href="https://ase.vu.nl/docs/framework/glossary/roverlib">About the roverlib</a>
  <br />
</div>
<br/>

**When building a service that runs on the Rover and should interface the ASE framework, you will most likely want to use a [roverlib](https://ase.vu.nl/docs/framework/glossary/roverlib). This is a C# template that incorporates [`roverlib-c-sharp`](https://github.com/VU-ASE/roverlib-c-sharp), meant to run on the Rover.**

## Running Locally

By default the Rovers come installed with roverlib-c-sharp in the `/home/debix/ase/roverlib-c-sharp` directory. However, if you are running it locally, you must edit the [project-file](./src/roverlib-c-sharp.csproj) to point the library to the location of roverlib-c-sharp on your system.

For example, download [roverlib-c-sharp](https://github.com/VU-ASE/roverlib-c-sharp/releases) and extract it to a directory on your local machine. Let's say the directory is saved under "/home/student/roverlib-c-sharp", then make sure to make the following edits to the [project-file](./src/roverlib-c-sharp.csproj)

``` xml
  <ItemGroup>
    <Reference Include="/home/student/roverlib-c-sharp/*.dll">
      <HintPath>/home/student/roverlib-c-sharp/%(Filename)%(Extension)</HintPath>
      <Private>true</Private>
    </Reference>
  </ItemGroup>
```

Note, that making this change will not work for running locally on the Rover, for that the path must be *"/home/debix/ase/roverlib-c-sharp/"*.


## Initialize a C# service

Instead of cloning this repository, it is recommended to initialize this C# service using `roverctl` as follows:

```bash
roverctl service init c-sharp --name c-sharp-example --source github.com/author/c-sharp-example
```

Read more about using `roverctl` to initialize services [here](https://ase.vu.nl/docs/framework/Software/rover/roverctl/usage#initialize-a-service).



