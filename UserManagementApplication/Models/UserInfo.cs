using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagementApplication.Models
{
    /// <summary>
    /// 사용자 정보 Model
    /// </summary>
    public class UserInfo : INotifyPropertyChanged
    {
        private string name = string.Empty,
            contact = string.Empty;
        private bool preparationStatus = false;
        private int age = 0,
            userId = 0;


        /// <summary>
        /// 사용자 정보 Property
        /// </summary>
        public bool PreparationStatus // 초기 생성 데이터 여부.
        {
            get { return preparationStatus; }
            set { preparationStatus = value; }
        }
        public string Name // 사용자 이름 Property
        { 
            get { return name; } 
            set { name = value; }
        }

        public string Contact // 사용자 연락처 Property
        {
            get { return contact; }
            set {  contact = value; }
        }

        public int Age // 사용자 나이 Property
        {
            get { return age; }
            set { age = value; }
        }

        public int UserId // Primary Key용 사용자 Id. AutoIncrement 처리.
        {
            get { return userId; }
            set { userId = value; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChaged(string  propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
