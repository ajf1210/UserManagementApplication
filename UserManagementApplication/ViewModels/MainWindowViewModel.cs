using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml.XPath;
using UserManagementApplication.Components;
using UserManagementApplication.Models;

namespace UserManagementApplication.ViewModels
{
    /// <summary>
    /// MainWindow View의 ViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Button 별 동작 Command
        /// </summary>
        public Command btnSearch { get; set; }
        public Command btnAdd { get; set; }
        public Command btnDelete { get; set; }

        /// <summary>
        /// 데이터 조회 및 추가를 위한 Property
        /// </summary>
        public string SearchName { get => searchName; set => searchName = value; }
        public string SearchContact { get => searchContact; set => searchContact = value; }
        public string AddName { get => addName; set => addName = value; }
        public string AddContact { get => addContact; set => addContact = value; }
        public int AddAge { get=> addAge; set => addAge = value; }

        /// <summary>
        /// DataGrid Binding Property
        /// </summary>
        public ObservableCollection<UserInfo> UserInfoList { get; set; } // 리스트 내 UserInfo 데이터 변경 시 View에 Notice 되도록 ObservableCollection 타입 사용.
        private object _lock = new object(); // Thread에서 UserInfoList Collection에 접근 할 때 잠금하기 위한 변수.

        private DBManager dbManager;
        private string searchName = string.Empty,
            searchContact = string.Empty,
            addName = string.Empty,
            addContact = string.Empty;
        private int addAge = 0;
        public MainWindowViewModel()
        {
            dbManager = new DBManager();

            //DataGrid ItemsSource 초기화.
            UserInfoList = new ObservableCollection<UserInfo>();

            // UIThread가 아닌 Thread에서 Collection에 액세스 하기 위한 처리를 하는 Method.
            // _lock 변수를 이용한 잠금처리를 통해 UI Thead와 동기화 후 View에 INotifyCollectionChanged를 통해 변경 알림.
            BindingOperations.EnableCollectionSynchronization(UserInfoList, _lock); 

            //Button 별 Command 처리 Method 할당.
            btnSearch = new Command(SearchProc, CanExecuteFunc);
            btnAdd = new Command(AddProc, CanExecuteFunc);
            btnDelete = new Command(DeleteProc, CanExecuteFunc);
        }

        /// <summary>
        /// 조회 기능 처리 Method
        /// </summary>
        /// <param name="obj"></param>
        private void SearchProc(object obj)
        {
            Task.Run(async () =>
            {
                Thread.Sleep(3000); //필수요구사항 3-f 충족을 위한 고의 지연시간
                await SearchAsync();
            });
        }

        /// <summary>
        /// 조회 기능 비동기 처리 함수.
        /// </summary>
        /// <returns></returns>
        private async Task SearchAsync()
        {
            //DB에서 이름과 전화번호를 기반으로 검색
            List<UserInfo> userInfos = (from u in dbManager.Users
                                        where u.Name.Contains(SearchName) && u.Contact.Contains(SearchContact)
                                        select u).ToList();
            
            //UserInfoList에 new ObservableCollection<UserInfo>(userInfos) 할 경우 Binding이 해제되기 때문에
            //UserInfoList.Clear() 후 조회된 내용을 순회하면서 Add.
            UserInfoList.Clear();
            foreach (UserInfo userInfo in userInfos)
            {
                UserInfoList.Add(userInfo);
            }
        }

        /// <summary>
        /// 추가 기능 처리 함수
        /// </summary>
        /// <param name="obj"></param>
        private void AddProc(object obj)
        {
            if (!CheckInput()) // 필수 입력 사항 확인.
                return;
            Task.Run(async () =>
            {
                Thread.Sleep(2000);//필수요구사항 3-f 충족을 위한 고의 지연시간
                await AddAsync();
            });
        }

        /// <summary>
        /// 추가 기능 비동기 처리 함수.
        /// </summary>
        /// <returns></returns>
        private async Task AddAsync()
        {
            // 데이터 추가용 Property를 통해 UserInfo Mode 객체 생성
            UserInfo userInfo = new UserInfo
            {
                Name = addName,
                Age = addAge,
                Contact = addContact,
                PreparationStatus = false
            };
            UserInfoList.Add(userInfo); // DataGrid UI 업데이트을 위한 ObservableCollection 업데이트
            dbManager.Users.Add(userInfo); // DB 삽입을 위한 DbSet 업데이트
            dbManager.SaveChanges(); // DbSet 변경사항 DB에 저장
        }

        /// <summary>
        /// 추가 기능 필수 입력사항 확인 Method
        /// </summary>
        /// <returns></returns>
        private bool CheckInput()
        {
            if (!addName.Equals("")
                && !addAge.Equals(0)
                && !addContact.Equals(""))
                return true;

            if (addName.Equals("")) MessageBox.Show("이름을 입력해주세요");
            else if (addAge == 0) MessageBox.Show("나이를 입력해주세요");
            else MessageBox.Show("연락처를 입력해주세요.");
            return false;
        }

        /// <summary>
        /// 삭제 기능 처리 함수.
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteProc(object obj)
        {
            UserInfo userInfo = (UserInfo)obj;
            if (userInfo.PreparationStatus)
            {
                MessageBox.Show("해당 데이터는 삭제할 수 없습니다.");
                return;
            }
            Task.Run(async () =>
            {
                Thread.Sleep(1000); //필수요구사항 3-f 충족을 위한 고의 지연시간
                await DeleteAsync(userInfo);
            });
        }

        /// <summary>
        /// 삭제 기능 비동기 처리 함수
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private async Task DeleteAsync(UserInfo userInfo)
        {
            UserInfoList.Remove(userInfo); //DataGrid UI 업데이트을 위한 ObservableCollection 업데이트
            dbManager.Users.Remove(userInfo); // DB 삽입을 위한 DbSet 업데이트
            dbManager.SaveChanges(); //DbSet 변경사항 DB에 저장
        }

        /// <summary>
        /// Command 실행 가능 여부 확인 Method
        /// 별도 기능 없음.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanExecuteFunc(object obj) 
        {
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
