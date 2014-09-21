﻿namespace Alienlab.DMT
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using Alienlab.DMT.Properties;

  public class DataSourceViewModel : INotifyPropertyChanged
  {
    [NotNull]
    protected readonly string FilePath = GetFilePath();

    [NotNull]
    private readonly IList<string> connectionStrings;

    private int selectedIndex;

    public DataSourceViewModel()
    {
      var collection = this.LoadConnectionStrings();
      Assert.IsNotNull(collection, "collection");

      collection.Add("<New>");

      this.connectionStrings = collection;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual int SelectedIndex
    {
      get
      {
        return this.selectedIndex;
      }
      set
      {
        if (value >= 0)
        {
          this.selectedIndex = value;
        }
      }
    }

    [NotNull]
    public virtual IList<string> ConnectionStrings
    {
      get
      {
        return this.connectionStrings;
      }
    }

    protected virtual void SaveConnectionStrings()
    {
      File.WriteAllLines(this.FilePath, this.ConnectionStrings.Take(this.ConnectionStrings.Count - 1));
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([NotNull] string propertyName)
    {
      Assert.ArgumentNotNull(propertyName, "propertyName");

      var handler = this.PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public virtual string Text
    {
      set
      {
        if (string.IsNullOrEmpty(value))
        {
          this.ConnectionStrings.RemoveAt(this.SelectedIndex);
          return;
        }

        if (this.SelectedIndex == this.ConnectionStrings.Count - 1)
        {
          if (value.Equals("<New>", StringComparison.OrdinalIgnoreCase))
          {
            return;
          }

          var index = this.ConnectionStrings.Count - 1;
          this.ConnectionStrings.Insert(index >= 0 ? index : 0, value);
        }
        else
        {
          if (this.ConnectionStrings[this.SelectedIndex].Equals(value, StringComparison.OrdinalIgnoreCase))
          {
            return;
          }

          this.ConnectionStrings[this.SelectedIndex] = value;
        }

        this.SaveConnectionStrings();
      }
    }

    protected ObservableCollection<string> LoadConnectionStrings()
    {
      if (File.Exists(this.FilePath))
      {
        return new ObservableCollection<string>(File.ReadAllLines(this.FilePath));
      }

      return new ObservableCollection<string>();
    }

    private static string GetFilePath()
    {
      var path = Environment.ExpandEnvironmentVariables("%APPDATA%\\Alienlab\\Database Management Tool\\DataSources.config");
      var dir = Path.GetDirectoryName(path);
      if (!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }

      return path;
    }
  }
}