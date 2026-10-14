using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

internal class MainWindow : Window
{
	private AppData _data;

	private System.Windows.Controls.ListBox _listBox;

	private System.Windows.Controls.Button _openBtn;

	private System.Windows.Controls.Button _deleteBtn;

	private Grid _contentPanel;

	private StackPanel _settingsPanel;

	private bool _settingsOpen = false;

	public MainWindow()
	{
		_data = LoadAll();
		Title = "Claude Code 启动器";
		Width = 560.0;
		Height = 440.0;
		ResizeMode = ResizeMode.CanResize;
		WindowStartupLocation = WindowStartupLocation.CenterScreen;
		MinWidth = 500.0;
		MinHeight = 380.0;
		Grid grid = (Grid)(Content = new Grid());
		Image image = new Image
		{
			Stretch = Stretch.UniformToFill
		};
		if (!string.IsNullOrEmpty(_data.BgImagePath) && File.Exists(_data.BgImagePath))
		{
			try
			{
				image.Source = new BitmapImage(new Uri(_data.BgImagePath));
			}
			catch
			{
			}
		}
		grid.Children.Add(image);
		Border element = new Border
		{
			Background = new SolidColorBrush(Color.FromArgb(180, 20, 20, 30))
		};
		grid.Children.Add(element);
		Grid grid3 = new Grid
		{
			RowDefinitions = 
			{
				new RowDefinition
				{
					Height = new GridLength(1.0, GridUnitType.Auto)
				},
				new RowDefinition
				{
					Height = new GridLength(1.0, GridUnitType.Star)
				},
				new RowDefinition
				{
					Height = new GridLength(1.0, GridUnitType.Auto)
				},
				new RowDefinition
				{
					Height = new GridLength(1.0, GridUnitType.Auto)
				}
			},
			Margin = new Thickness(30.0, 20.0, 30.0, 25.0)
		};
		grid.Children.Add(grid3);
		Grid grid4 = new Grid
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 12.0)
		};
		TextBlock element2 = new TextBlock
		{
			Text = "Claude Code 启动器",
			FontSize = 18.0,
			FontWeight = FontWeights.Bold,
			Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 240)),
			HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Center
		};
		grid4.Children.Add(element2);
		System.Windows.Controls.Button gearBtn = new System.Windows.Controls.Button
		{
			Content = "⚙",
			FontSize = 20.0,
			Width = 36.0,
			Height = 36.0,
			HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
			Background = Brushes.Transparent,
			BorderThickness = new Thickness(0.0),
			Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 200)),
			Cursor = System.Windows.Input.Cursors.Hand,
			ToolTip = "设置"
		};
		gearBtn.Click += (object s, RoutedEventArgs e) =>
		{
			ToggleSettings();
		};
		gearBtn.MouseEnter += (object s, System.Windows.Input.MouseEventArgs e) =>
		{
			AnimateTo(gearBtn, "Foreground.Opacity", null, gearBtn.Foreground, new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue)), 0.15);
		};
		gearBtn.MouseLeave += (object s, System.Windows.Input.MouseEventArgs e) =>
		{
			AnimateTo(gearBtn, "Foreground.Opacity", null, gearBtn.Foreground, new SolidColorBrush(Color.FromRgb(180, 180, 200)), 0.15);
		};
		grid4.Children.Add(gearBtn);
		Grid.SetRow(grid4, 0);
		grid3.Children.Add(grid4);
		_settingsPanel = CreateSettingsPanel();
		_settingsPanel.Visibility = Visibility.Collapsed;
		Grid.SetRow(_settingsPanel, 1);
		grid3.Children.Add(_settingsPanel);
		_contentPanel = new Grid();
		_contentPanel.RowDefinitions.Add(new RowDefinition
		{
			Height = new GridLength(1.0, GridUnitType.Star)
		});
		_contentPanel.RowDefinitions.Add(new RowDefinition
		{
			Height = new GridLength(1.0, GridUnitType.Auto)
		});
		Border border = new Border
		{
			Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
			CornerRadius = new CornerRadius(8.0),
			Padding = new Thickness(10.0),
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};
		_listBox = new System.Windows.Controls.ListBox
		{
			FontSize = 12.0,
			FontFamily = new FontFamily("Segoe UI"),
			Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 240)),
			Background = Brushes.Transparent,
			BorderThickness = new Thickness(0.0),
			HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch
		};
		if (_data.Workspaces.Count == 0)
		{
			_listBox.Items.Add("（暂无历史记录）");
			_listBox.IsEnabled = false;
		}
		else
		{
			foreach (string workspace in _data.Workspaces)
			{
				_listBox.Items.Add(workspace);
			}
			_listBox.SelectedIndex = 0;
		}
		_listBox.MouseDoubleClick += (object s, MouseButtonEventArgs e) =>
		{
			if (_listBox.SelectedItem != null && _data.Workspaces.Count > 0)
			{
				Launch(_listBox.SelectedItem.ToString());
			}
		};
		_listBox.SelectionChanged += (object s, SelectionChangedEventArgs e) =>
		{
			UpdateButtonStates();
		};
		border.Child = _listBox;
		Grid.SetRow(border, 0);
		_contentPanel.Children.Add(border);
		WrapPanel wrapPanel = new WrapPanel
		{
			HorizontalAlignment = System.Windows.HorizontalAlignment.Center
		};
		_openBtn = MakeBtn("启动 Claude Code", primary: true, () =>
		{
			if (_listBox.SelectedItem != null && _data.Workspaces.Count > 0)
			{
				Launch(_listBox.SelectedItem.ToString());
			}
		});
		wrapPanel.Children.Add(_openBtn);
		System.Windows.Controls.Button element3 = MakeBtn("选择新工作区...", primary: false, () =>
		{
			using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
			{
				Description = "选择新的工作区文件夹",
				ShowNewFolderButton = true
			};
			if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
			{
				Launch(folderBrowserDialog.SelectedPath);
			}
		});
		wrapPanel.Children.Add(element3);
		_deleteBtn = MakeBtn("删除选中", primary: false, () =>
		{
			if (_listBox.SelectedIndex >= 0 && _data.Workspaces.Count > 0)
			{
				_data.Workspaces.RemoveAt(_listBox.SelectedIndex);
				SaveHistory(_data.Workspaces);
				_listBox.Items.RemoveAt(_listBox.SelectedIndex);
				if (_data.Workspaces.Count == 0)
				{
					_listBox.Items.Add("（暂无历史记录）");
					_listBox.IsEnabled = false;
				}
				UpdateButtonStates();
			}
		});
		wrapPanel.Children.Add(_deleteBtn);
		Grid.SetRow(wrapPanel, 1);
		_contentPanel.Children.Add(wrapPanel);
		Grid.SetRow(_contentPanel, 1);
		grid3.Children.Add(_contentPanel);
		TextBlock element4 = new TextBlock
		{
			Text = "历史记录: " + AppData.HistoryFile,
			FontSize = 10.0,
			Foreground = new SolidColorBrush(Color.FromArgb(160, 180, 180, 190)),
			HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
			Margin = new Thickness(0.0, 6.0, 0.0, 0.0)
		};
		Grid.SetRow(element4, 2);
		grid3.Children.Add(element4);
		UpdateButtonStates();
		Opacity = 0.0;
		Loaded += (object s, RoutedEventArgs e) =>
		{
			DoubleAnimation animation = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(0.35));
			BeginAnimation(UIElement.OpacityProperty, animation);
		};
	}

	private StackPanel CreateSettingsPanel()
	{
		StackPanel stackPanel = new StackPanel();
		stackPanel.Margin = new Thickness(0.0, 0.0, 0.0, 14.0);
		StackPanel stackPanel2 = stackPanel;
		Border border = new Border();
		border.Background = new SolidColorBrush(Color.FromArgb(140, 10, 10, 25));
		border.CornerRadius = new CornerRadius(8.0);
		border.Padding = new Thickness(20.0);
		Border border2 = border;
		StackPanel stackPanel3 = (StackPanel)(border2.Child = new StackPanel());
		stackPanel2.Children.Add(border2);
		TextBlock textBlock = new TextBlock();
		textBlock.Text = "设置";
		textBlock.FontSize = 16.0;
		textBlock.FontWeight = FontWeights.Bold;
		textBlock.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 220));
		textBlock.Margin = new Thickness(0.0, 0.0, 0.0, 12.0);
		TextBlock element = textBlock;
		stackPanel3.Children.Add(element);
		Grid grid = new Grid();
		grid.Margin = new Thickness(0.0, 0.0, 0.0, 10.0);
		Grid grid2 = grid;
		grid2.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Auto)
		});
		grid2.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, GridUnitType.Star)
		});
		TextBlock textBlock2 = new TextBlock();
		textBlock2.Text = "背景图:";
		textBlock2.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 200));
		textBlock2.VerticalAlignment = VerticalAlignment.Center;
		textBlock2.Margin = new Thickness(0.0, 0.0, 10.0, 0.0);
		textBlock2.FontSize = 13.0;
		TextBlock element2 = textBlock2;
		Grid.SetColumn(element2, 0);
		grid2.Children.Add(element2);
		TextBlock pathText = new TextBlock
		{
			Text = (string.IsNullOrEmpty(_data.BgImagePath) ? "(未设置)" : _data.BgImagePath),
			Foreground = new SolidColorBrush(Color.FromRgb(140, 150, 180)),
			TextTrimming = TextTrimming.CharacterEllipsis,
			FontSize = 12.0,
			VerticalAlignment = VerticalAlignment.Center
		};
		Grid.SetColumn(pathText, 1);
		grid2.Children.Add(pathText);
		stackPanel3.Children.Add(grid2);
		WrapPanel wrapPanel = new WrapPanel();
		System.Windows.Controls.Button button = new System.Windows.Controls.Button();
		button.Content = "选择背景图片...";
		button.FontSize = 13.0;
		button.Width = 130.0;
		button.Height = 32.0;
		button.Background = new SolidColorBrush(Color.FromRgb(60, 60, 100));
		button.Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 240));
		button.BorderThickness = new Thickness(0.0);
		button.Cursor = System.Windows.Input.Cursors.Hand;
		button.Margin = new Thickness(0.0, 0.0, 10.0, 0.0);
		System.Windows.Controls.Button button2 = button;
		button2.Click += (object s, RoutedEventArgs e) =>
		{
			Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
			{
				Title = "选择背景图片",
				Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				_data.BgImagePath = openFileDialog.FileName;
				SaveSettings(_data.BgImagePath);
				pathText.Text = _data.BgImagePath;
				try
				{
					Image image = (Image)((Grid)Content).Children[0];
					image.Source = new BitmapImage(new Uri(_data.BgImagePath));
				}
				catch
				{
				}
			}
		};
		wrapPanel.Children.Add(button2);
		System.Windows.Controls.Button button3 = new System.Windows.Controls.Button();
		button3.Content = "清除背景图";
		button3.FontSize = 13.0;
		button3.Width = 110.0;
		button3.Height = 32.0;
		button3.Background = new SolidColorBrush(Color.FromRgb(80, 50, 50));
		button3.Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 240));
		button3.BorderThickness = new Thickness(0.0);
		button3.Cursor = System.Windows.Input.Cursors.Hand;
		System.Windows.Controls.Button button4 = button3;
		button4.Click += (object s, RoutedEventArgs e) =>
		{
			_data.BgImagePath = "";
			SaveSettings("");
			pathText.Text = "(未设置)";
			try
			{
				((Image)((Grid)Content).Children[0]).Source = null;
			}
			catch
			{
			}
		};
		wrapPanel.Children.Add(button4);
		stackPanel3.Children.Add(wrapPanel);
		return stackPanel2;
	}

	private void ToggleSettings()
	{
		_settingsOpen = !_settingsOpen;
		if (_settingsOpen)
		{
			_settingsPanel.Visibility = Visibility.Visible;
			_contentPanel.Visibility = Visibility.Collapsed;
			_settingsPanel.RenderTransform = new TranslateTransform(0.0, -20.0);
			AnimateTransform(_settingsPanel, TranslateTransform.YProperty, -20.0, 0.0, 0.25);
		}
		else
		{
			_settingsPanel.Visibility = Visibility.Collapsed;
			_contentPanel.Visibility = Visibility.Visible;
			_contentPanel.RenderTransform = new TranslateTransform(0.0, -20.0);
			AnimateTransform(_contentPanel, TranslateTransform.YProperty, -20.0, 0.0, 0.25);
		}
	}

	private System.Windows.Controls.Button MakeBtn(string text, bool primary, Action onClick)
	{
		System.Windows.Controls.Button btn = new System.Windows.Controls.Button
		{
			Content = text,
			FontSize = 13.0,
			Width = 145.0,
			Height = 38.0,
			Margin = new Thickness(4.0),
			Cursor = System.Windows.Input.Cursors.Hand,
			BorderThickness = new Thickness(0.0),
			Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 245)),
			Background = (primary ? new SolidColorBrush(Color.FromRgb(70, 100, 200)) : new SolidColorBrush(Color.FromRgb(55, 55, 80)))
		};
		Style style = new Style(typeof(System.Windows.Controls.Button));
		ControlTemplate controlTemplate = new ControlTemplate(typeof(System.Windows.Controls.Button));
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(Border));
		frameworkElementFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(8.0));
		frameworkElementFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(System.Windows.Controls.Control.BackgroundProperty));
		frameworkElementFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0.0));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ContentPresenter));
		frameworkElementFactory2.SetValue(FrameworkElement.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center);
		frameworkElementFactory2.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		controlTemplate.VisualTree = frameworkElementFactory;
		style.Setters.Add(new Setter(System.Windows.Controls.Control.TemplateProperty, controlTemplate));
		btn.Style = style;
		SolidColorBrush solidColorBrush = (SolidColorBrush)btn.Background;
		Color baseColor = solidColorBrush.Color;
		Color hoverColor = Color.FromArgb(baseColor.A, (byte)Math.Min(255, baseColor.R + 35), (byte)Math.Min(255, baseColor.G + 35), (byte)Math.Min(255, baseColor.B + 35));
		btn.MouseEnter += (object s, System.Windows.Input.MouseEventArgs e) =>
		{
			ColorAnimation animation = new ColorAnimation(hoverColor, TimeSpan.FromSeconds(0.12));
			btn.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
		};
		btn.MouseLeave += (object s, System.Windows.Input.MouseEventArgs e) =>
		{
			ColorAnimation animation = new ColorAnimation(baseColor, TimeSpan.FromSeconds(0.12));
			btn.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
		};
		btn.Click += (object s, RoutedEventArgs e) =>
		{
			onClick();
		};
		return btn;
	}

	private void UpdateButtonStates()
	{
		bool flag = _data.Workspaces.Count > 0 && _listBox.SelectedIndex >= 0;
		_openBtn.IsEnabled = flag;
		_openBtn.Opacity = (flag ? 1.0 : 0.5);
		_deleteBtn.IsEnabled = flag;
		_deleteBtn.Opacity = (flag ? 1.0 : 0.5);
	}

	private void AnimateTo(UIElement target, string prop, DependencyProperty dp, object from, object to, double sec)
	{
		if (dp == null)
		{
			return;
		}
		try
		{
			double toValue = (int)((to is SolidColorBrush { Color: var color }) ? color.A : byte.MaxValue);
			DoubleAnimation doubleAnimation = new DoubleAnimation(toValue, TimeSpan.FromSeconds(sec));
		}
		catch
		{
		}
	}

	private void AnimateTransform(UIElement target, DependencyProperty prop, double from, double to, double sec)
	{
		DoubleAnimation doubleAnimation = new DoubleAnimation(from, to, TimeSpan.FromSeconds(sec));
		doubleAnimation.EasingFunction = new QuadraticEase
		{
			EasingMode = EasingMode.EaseOut
		};
		DoubleAnimation animation = doubleAnimation;
		if (target.RenderTransform is TranslateTransform translateTransform)
		{
			translateTransform.BeginAnimation(prop, animation);
		}
	}

	private void Launch(string folder)
	{
		_data.Workspaces.RemoveAll((string p) => string.Equals(p, folder, StringComparison.OrdinalIgnoreCase));
		_data.Workspaces.Insert(0, folder);
		if (_data.Workspaces.Count > 10)
		{
			_data.Workspaces = _data.Workspaces.Take(10).ToList();
		}
		SaveHistory(_data.Workspaces);
		try
		{
			// 在 Windows Terminal 中打开（老 conhost 无法正确渲染 Claude Code 的界面）
			string script = "Set-Location -LiteralPath " + PsQuote(folder) + "; Write-Host ('工作区: ' + " + PsQuote(folder) + ") -ForegroundColor Green; Write-Host '正在启动 Claude Code...' -ForegroundColor Yellow; claude";
			string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
			string wt = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "WindowsApps", "wt.exe");
			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				FileName = (File.Exists(wt) ? wt : "wt.exe"),
				Arguments = "powershell.exe -NoExit -EncodedCommand " + encoded,
				UseShellExecute = true
			};
			Process.Start(processStartInfo);
		}
		catch (Exception ex)
		{
			System.Windows.MessageBox.Show("启动失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private static string PsQuote(string s)
	{
		return "'" + s.Replace("'", "''") + "'";
	}

	private static List<string> LoadHistoryText()
	{
		try
		{
			if (File.Exists(AppData.HistoryFile))
			{
				return (from l in File.ReadAllLines(AppData.HistoryFile)
					select l.Trim() into l
					where l != "" && Directory.Exists(l)
					select l).Take(10).ToList();
			}
		}
		catch
		{
		}
		return new List<string>();
	}

	private static void SaveHistory(List<string> list)
	{
		try
		{
			Directory.CreateDirectory(AppData.DataDir);
			File.WriteAllLines(AppData.HistoryFile, list);
		}
		catch
		{
		}
	}

	private static string LoadSettingsText()
	{
		try
		{
			if (File.Exists(AppData.SettingsFile))
			{
				return File.ReadAllText(AppData.SettingsFile).Trim();
			}
		}
		catch
		{
		}
		return "";
	}

	private static void SaveSettings(string bgPath)
	{
		try
		{
			Directory.CreateDirectory(AppData.DataDir);
			File.WriteAllText(AppData.SettingsFile, bgPath);
		}
		catch
		{
		}
	}

	private static AppData LoadAll()
	{
		AppData appData = new AppData();
		appData.Workspaces = LoadHistoryText();
		appData.BgImagePath = LoadSettingsText();
		return appData;
	}
}
