﻿namespace Alienlab.DMT
{
  using System;
  using System.Collections.Specialized;
  using System.Dynamic;
  using System.IO;
  using System.Linq;
  using System.Windows.Input;
  using Microsoft.Win32;

  public class AttachDatabaseViewModel
  {
    private string physicalPath;
    public DataSourceViewModel DataSourceViewModel { get; set; }

    public ICommand DoAttach { get; set; }

    public string LogicalName { get; set; }

    public string PhysicalPath
    {
      get
      {
        return this.physicalPath ?? Environment.GetCommandLineArgs().Skip(1).FirstOrDefault() ?? (this.physicalPath = this.ChooseDatabaseFile());
      }
    }

    private string ChooseDatabaseFile()
    {
      var openFileDialog = new OpenFileDialog
      {
        AddExtension = true,
        CheckFileExists = true,
        CheckPathExists = true,
        Filter = "MS SQL Server Database File (*.MDF)|*.mdf",
        InitialDirectory = Path.GetPathRoot(Environment.ExpandEnvironmentVariables(Environment.SystemDirectory)),
        Multiselect = false, // we do not support multiple mdf at once for now
        Title = "Choose MS SQL Server Database .MDF File"
      };

      var result = openFileDialog.ShowDialog();
      
      return result.HasValue && result.Value ? openFileDialog.FileName : null;
    }
  }
}