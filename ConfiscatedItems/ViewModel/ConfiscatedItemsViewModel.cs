using ConfiscatedItems.Data;
using ConfiscatedItems.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;

namespace ConfiscatedItems.ViewModel
{
    public class ConfiscatedItemsViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ConfiscatedItemsContext Context = new ConfiscatedItemsContext();

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(TxtItemName):
                        if (string.IsNullOrEmpty(TxtItemName))
                            return "Angiv et navn";
                        break;
                    case nameof(Category):
                        if (string.IsNullOrEmpty(Category))
                            return "Angiv en Kategori";
                        break;
                        // Define validation for other properties...
                }

                return null; // No error
            }
        }

        [Required(ErrorMessage = "Angiv et navn")]
        private string txtItemName;
        public string TxtItemName
        {
            get { return txtItemName; }
            set
            {
                if (txtItemName != value)
                {
                    txtItemName = value;
                    OnPropertyChanged(nameof(txtItemName));
                }
            }
        }

        private string txtDescription;
        public string TxtDescription
        {
            get { return txtDescription; }
            set
            {
                if (txtDescription != value)
                {
                    txtDescription = value;
                    OnPropertyChanged(nameof(txtDescription));
                }
            }
        }

        [Required(ErrorMessage ="Angiv en kategori")]
        private string category;
        public string Category
        {
            get { return category; }
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged(nameof(category));
                }
            }
        }

        private List<Item> items;
        public List<Item> Items
        {
            get { return items; }
            set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged(nameof(items));
                }
            }
        }

        private Item selectedGridItem;
        public Item SelectedGridItem
        {
            get { return selectedGridItem; }
            set
            {
                if (selectedGridItem != value)
                {
                    selectedGridItem = value;
                    OnPropertyChanged(nameof(selectedGridItem));
                }
            }
        }


        private ICommand _clickCommand;
        public ICommand CreateItemCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => CreateItem(), () => CanExecute));
            }
        }
        private ICommand _getAllCommand;
        public ICommand GetAllItemsCommand
        {
            get
            {
                return _getAllCommand ?? (_getAllCommand = new CommandHandler(() => GetAllItems(), () => CanExecute));
            }
        }
        private ICommand _updateItemCommand;
        public ICommand UpdateItemCommand
        {
            get
            {
                return _updateItemCommand ?? (_updateItemCommand = new CommandHandler(() => UpdateItem(), () => CanExecute));
            }
        }
        private ICommand _getItemCommand;
        public ICommand GetItemCommand
        {
            get
            {
                return _getItemCommand ?? (_getItemCommand = new CommandHandler(() => GetItem(), () => CanExecute));
            }
        }
        private ICommand _deleteItemCommand;


        public ICommand DeleteItemCommand
        {
            get
            {
                return _deleteItemCommand ?? (_deleteItemCommand = new CommandHandler(() => DeleteItem(), () => CanExecute));
            }
        }
        public bool CanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
                return true;
            }
        }

        public ConfiscatedItemsViewModel()
        {
            GetAllItems();
        }

        public void CreateItem()
        {
            Item item = new Item()
            {
                Name = TxtItemName,
                Description = TxtDescription,
                Category = Category,
                CreateDate = DateTime.Now
            };
            Context.Items.Add(item);
            Context.SaveChanges();

            //Reset text indput boxes
            TxtItemName = "";
            TxtDescription = "";
            Category = "";

            GetItem();
        }

        public List<Item> GetAllItems()
        {
            return Items = Context.Items.ToList();
        }

        public void UpdateItem()
        {
            if (SelectedGridItem != null)
            {
                var result = Context.Items.SingleOrDefault(i => i.Id == SelectedGridItem.Id);
                if (result != null)
                {
                    if ((!string.IsNullOrEmpty(TxtItemName) && TxtItemName != result.Name) ||
                        (!string.IsNullOrEmpty(TxtDescription) && TxtDescription != result.Description) ||
                        (!string.IsNullOrEmpty(Category) && Category != result.Category))
                    {
                        if (!string.IsNullOrEmpty(TxtItemName))
                        {
                            result.Name = TxtItemName;
                        }
                        if (!string.IsNullOrEmpty(TxtDescription))
                        {
                            result.Description = TxtDescription;
                        }
                        if (!string.IsNullOrEmpty(Category))
                        {
                            result.Category = Category;
                        }
                    }
                    Context.SaveChanges();
                    GetItem();

                }
            }
        }

        public void DeleteItem()
        {
            if (selectedGridItem != null)
            {
                try
                {
                    Context.Remove(SelectedGridItem);
                    Context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            GetAllItems();
        }

        public void GetItem()
        {
            var query = from item in Context.Items
                        where (string.IsNullOrEmpty(TxtItemName) || EF.Functions.Like(item.Name, $"%{TxtItemName}%"))
                           && (string.IsNullOrEmpty(TxtDescription) || EF.Functions.Like(item.Description, $"%{TxtDescription}%"))
                           && (string.IsNullOrEmpty(Category) || EF.Functions.Like(item.Category, $"%{Category}%"))
                        select item;

            var matchingItems = query.ToList(); // Retrieve all matching items

            Items = matchingItems;
        }
    }
}
