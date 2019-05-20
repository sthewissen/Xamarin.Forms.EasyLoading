<img src="https://github.com/sthewissen/Xamarin.Forms.EasyLoading/blob/master/images/icon.png" width="150px" />

# Xamarin.Forms.EasyLoading
A collection of attached properties that let you specify a loading state view for any of your existing layouts.

[![Build status](https://sthewissen.visualstudio.com/EasyLoading/_apis/build/status/EasyLoading-CI)]() ![](https://img.shields.io/nuget/vpre/Xamarin.Forms.EasyLoading.svg)

## Why EasyLoading?

Have you ever had a piece of XAML code that didn't produce the layout you expected? Did you change background colors on certain elements to get an idea of where they are positioned? Admit it, you have and pretty much all of us have at some point. Either way, this is the package for you! It adds a very colorful debug mode to each of your `ContentPage`s that lets you immediately see where all of your elements are located!

<img src="https://raw.githubusercontent.com/sthewissen/Xamarin.Forms.EasyLoading/master/images/sample.png" />

## How to use it?

The project is up on NuGet at the following URL:

https://www.nuget.org/packages/Xamarin.Forms.EasyLoading

Install this package into your shared project. There is no need to install it in your platform specific projects. After that you're good to go! Simply add the namespace declaration and the new `LoadingLayout` related attached properties should be available to you!

```
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
   xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
   xmlns:local="clr-namespace:Xamarin.Forms.EasyLoading; assembly=Xamarin.Forms.EasyLoading" 
   x:Class="SampleApp.MainPage">

   <Grid ll:LoadingLayout.IsLoading="{Binding IsFullscreenLoading}">
      <ll:LoadingLayout.LoadingTemplate>
         <DataTemplate>
            <Grid BackgroundColor="White">
               <StackLayout VerticalOptions="Center" HorizontalOptions="Center">
                  <ActivityIndicator Color="#1abc9c" IsRunning="{Binding IsFullscreenLoading}" />
                  <Label Text="Loading..." HorizontalOptions="Center" />
               </StackLayout>
            </Grid>
         </DataTemplate>
      </ll:LoadingLayout.LoadingTemplate>      
  
     ...
     
  </Grid>
  
</ContentPage>
```

## What can I do with it?

Your imagination is the only limit to what you can do. You can use the control to recreate your layout and make a skeleton loader our of it. Or you can just use a simple `Label` to indicate that you're loading something. The following parts are what make this thing tick:

| Property | What it does | Extra info |
| ------ | ------ | ------ |
| `IsLoading` | Bind this to a property that indicates when to show/hide the loader. | |
| `LoadingTemplate` | A data template that contains what is shown when loading. | A ```DataTemplate``` object. |
| `LoadingTemplateSelector` | A template selector to dynamically show a specific loader. | A ```DataTemplateSelector``` object. |
| `RepeatCount` | Repeats the `LoadingTemplate` by a given amount. | Ideal to use to show a list of items loading. |
