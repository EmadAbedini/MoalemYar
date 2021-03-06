﻿/****************************** ghost1372.github.io ******************************\
*	Module Name:	AddUser.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 2, 10:56 ب.ظ
*
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUserView : UserControl
    {
        internal static AddUserView main;
        private List<DataClass.Tables.User> _initialCollection;

        public AddUserView()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;
            getUser();
        }

        #region Query

        private void getUser()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Users.ToList();
                    _initialCollection = query;
                    if (query.Any())
                    {
                        dataGrid.ItemsSource = query;
                    }
                    else
                    {
                        dataGrid.ItemsSource = null;
                        MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.No_Data_KEY, param: "User");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteUser(long id)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteUser = db.Users.Find(id);
                db.Users.Remove(DeleteUser);
                db.SaveChanges();
            }
        }

        private void updateUser(long id, string Username, string Password)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditUser = db.Users.Find(id);
                EditUser.Username = Username;
                EditUser.Password = Password;

                db.SaveChanges();
            }
        }

        private void addUser(string Username, string Password)
        {
            using (var db = new DataClass.myDbContext())
            {
                var User = new DataClass.Tables.User();
                User.Username = Username;
                User.Password = Password;

                db.Users.Add(User);

                db.SaveChanges();
            }
        }

        #endregion Query

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ClearScreen();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                txtUsername.Text = selectedItem.Username;
                txtPassword.Text = selectedItem.Password;
                txtPasswordAg.Text = selectedItem.Password;
            }
            catch (Exception)
            {
            }
        }

        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtPassword.Text != txtPasswordAg.Text)
                {
                    MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.Same_Password_KEY);
                }
                else
                {
                    dynamic selectedItem = dataGrid.SelectedItems[0];
                    long id = selectedItem.Id;
                    updateUser(id, txtUsername.Text.ToLower(), txtPassword.Text.ToLower());
                    MainWindow.main.showGrowlNotification(AppVariable.Update_Data_KEY, true, txtUsername.Text, "نام کاربری");
                    getUser();
                }
            }
            catch (Exception)
            {
                MainWindow.main.showGrowlNotification(AppVariable.Update_Data_KEY, false, txtUsername.Text, "نام کاربری");
            }
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.UnselectAll();
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtPasswordAg.Text = string.Empty;
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGrid.ItemsSource != null)
            {
                if (txtEditSearch.Text != string.Empty)
                    dataGrid.ItemsSource = _initialCollection.Where(x => x.Username.Contains(txtEditSearch.Text)).Select(x => x);
                else
                    dataGrid.ItemsSource = _initialCollection.Select(x => x);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAddUsername.Text == string.Empty || txtAddPassword.Password == string.Empty || txtAddPasswordAg.Password == string.Empty)
            {
                MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.Fill_All_Data_KEY);
            }
            else if (txtAddPassword.Password != txtAddPasswordAg.Password)
            {
                MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.Same_Password_KEY);
            }
            else
            {
                try
                {
                    addUser(txtAddUsername.Text.ToLower(), txtAddPassword.Password.ToLower());
                    MainWindow.main.showGrowlNotification(AppVariable.Add_Data_KEY, true, txtAddUsername.Text, "نام کاربری");
                    txtAddUsername.Text = string.Empty;
                    txtAddPassword.Password = string.Empty;
                    txtAddPasswordAg.Password = string.Empty;
                    txtAddPassword.Focus();
                    getUser();
                }
                catch (Exception)
                {
                    MainWindow.main.showGrowlNotification(AppVariable.Add_Data_KEY, false, txtAddUsername.Text, "نام کاربری");
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.Delete_Confirm_KEY, param: new[] { txtUsername.Text, "کاربر" });
        }

        public void deleteUser()
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                long id = selectedItem.Id;
                deleteUser(id);
                MainWindow.main.showGrowlNotification(AppVariable.Deleted_KEY, true, txtUsername.Text, "نام کاربری");
                getUser();
            }
            catch (Exception)
            {
                MainWindow.main.showGrowlNotification(AppVariable.Deleted_KEY, false, txtUsername.Text, "نام کاربری");
            }
        }
    }
}