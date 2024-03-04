using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using AngleSharp;
using AngleSharp.Dom;

namespace course
{
    class AppVM : INotifyPropertyChanged
    {

        public AppVM()
        {
            Categories = new BindingList<partsCategory>();
            Parts = new BindingList<part>();
        }

        public BindingList<partsCategory> Categories { get; set; }
        public BindingList<part> Parts { get; set; }


        private partsCategory selectedCategory;

        public partsCategory SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }

        private part selectedPart;

        public part SelectedPart
        {
            get { return selectedPart; }
            set
            {
                selectedPart = value;
                OnPropertyChanged("SelectedPart");
            }
        }


        private RelayCommand categoryDetailsCommand;
        public RelayCommand CategoryDetailsCommand
        {
            get
            {
                return categoryDetailsCommand ??
                    (categoryDetailsCommand = new RelayCommand(obj =>
                    {



                        CategoryDetailsWindow dWindow = new CategoryDetailsWindow();
                        dWindow.DataContext = this;
                        if (SelectedCategory != null)
                        {
                            ParseParts();
                            dWindow.Show();
                        }

                    }));
            }
        }

        private RelayCommand partDetailsCommand;
        public RelayCommand PartDetailsCommand
        {
            get
            {
                return partDetailsCommand ??
                    (partDetailsCommand = new RelayCommand(obj =>
                    {
                        if (SelectedPart != null)
                            MessageBox.Show($"{SelectedPart.Name} \n{SelectedPart.Price} Рублей \nНаличие:{SelectedPart.Availability} \n\n{SelectedPart.Description}");
                    }));
            }
        }

        private RelayCommand addToFeatured;
        public RelayCommand AddToFeatured
        {
            get
            {
                return addToFeatured ??
                    (addToFeatured = new RelayCommand(obj =>
                    {
                        if (SelectedPart != null)
                        {
                            using (dataContext db = new dataContext())
                            {
                                part p = new part
                                {
                                    Name = SelectedPart.Name,
                                    Price = SelectedPart.Price,
                                    Availability = SelectedPart.Availability,
                                    Description = SelectedPart.Description,
                                    ImageUrl = selectedPart.ImageUrl,
                                    DateAdded = selectedPart.DateAdded
                                };
                                if (db.Parts.FirstOrDefault(o => o.Name == p.Name && o.Price == p.Price) == null)
                                {
                                    db.Parts.Add(p);
                                    db.SaveChanges();
                                    MessageBox.Show($"Деталь {SelectedPart.Name} была добавлена в избранное");
                                }
                                else
                                    MessageBox.Show("Деталь уже добавлена в избранное");
                            }
                        }
                        else
                            MessageBox.Show("Для начала выберите деталь!");
                    }
                    ));
            }
        }

        private RelayCommand showFeatured;
        public RelayCommand ShowFeatured
        {
            get
            {
                return showFeatured ??
                    (showFeatured = new RelayCommand(obj =>
                    {
                        FeaturedList fWindow = new FeaturedList();
                        fWindow.DataContext = this;
                        DBGetFeaturedList();
                        fWindow.Show();
                    }
                    ));
            }
        }

        private RelayCommand deleteFromFeatured;
        public RelayCommand DeleteFromFeatured
        {
            get
            {
                return deleteFromFeatured ??
                    (deleteFromFeatured = new RelayCommand(obj =>
                    {
                        if (SelectedPart != null)
                            DBDeleteFeaturedItem();
                    }
                    ));
            }
        }

        private RelayCommand clearFeaturedList;
        public RelayCommand ClearFeaturedList
        {
            get
            {
                return clearFeaturedList ??
                    (clearFeaturedList = new RelayCommand(obj =>
                    {
                        DBClearFeaturedList();
                        MessageBox.Show("Избранное было очищено");
                    }
                    ));
            }
        }

        private RelayCommand generateReportCommand;
        public RelayCommand GenerateReportCommand
        {
            get
            {
                return generateReportCommand ??
                    (generateReportCommand = new RelayCommand(obj =>
                    {
                        if (SelectedPart != null)
                        {
                            GenerateReport();
                            MessageBox.Show("Отчет был сформирован");
                        }
                        else
                            MessageBox.Show("Сначала выберите деталь95");
                    }
                    ));
            }
        }

        private RelayCommand generateChartCommand;
        public RelayCommand GenerateChartCommand
        {
            get
            {
                return generateChartCommand ??
                    (generateChartCommand = new RelayCommand(obj =>
                    {
                        GenerateChart();
                        MessageBox.Show("График был сформирован");
                    }));
            }
        }

        public void DBDeleteFeaturedItem()
        {
            using (dataContext db = new dataContext())
            {
                var p = db.Parts.FirstOrDefault(o => o.Name == SelectedPart.Name && o.Price == SelectedPart.Price);
                if (p != null)
                {
                    db.Parts.Remove(p);
                }
                db.SaveChanges();
            }
            DBGetFeaturedList();
        }
        public void DBClearFeaturedList()
        {
            using (dataContext db = new dataContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Parts");
            }
            Parts.Clear();
            OnPropertyChanged("Parts");
        } 

        public void DBGetFeaturedList()
        {
            Parts.Clear();
            using (dataContext db = new dataContext())
            {
                foreach(part p in db.Parts)
                {
                    Parts.Add(p);
                }
            }
            OnPropertyChanged("Parts");
        }
        public async void ParseParts()
        {
            Parts.Clear();
            var parser = new Parser();
            Parts = await parser.ParseParts(SelectedCategory.Url);
            OnPropertyChanged("Parts");
        }
        public async void ParseCategories()
        {
            var parser = new Parser();
            string url = "https://toybike.ru/catalog/zapchasti/bmx_zapchasti/";
            Categories = await parser.ParseCategories(url);
            OnPropertyChanged("Categories");
        }



        private void GenerateChart()
        {
            var excel = new ExcelGenerator();
            excel.GenerateChart(Parts);
            excel.AddChartToReport();
        }



        private void GenerateReport()
        {
            var word = new WordGenerator(@"C:\Users\Пользователь\source\repos\course\course\template.doc");

            var items = new Dictionary<string, string>
            {
                {"<PARTNAME>", SelectedPart.Name },
                {"<PARTNAMEFULL>", SelectedPart.Name },
                {"<PRICE>", SelectedPart.Price },
                {"<AVAILABLE>", SelectedPart.Availability },
                {"<DATE>", SelectedPart.DateAdded.ToString() }
            };
            word.Process(items);
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
