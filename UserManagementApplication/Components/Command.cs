using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UserManagementApplication.Components
{
    /// <summary>
    /// Button Click Event 바인딩용 Command
    /// 생성자로 ExecuteMethod와 CanExecuteMethod에 해당하는
    /// 함수를 전달받아 실행.
    /// </summary>
    public class Command : ICommand
    {
        Action<object> ExecuteMethod;
        Func<object, bool> CanExecuteMethod;

        public Command(Action<object> execute_Method,
            Func<object, bool> canexecute_Method)
        {
            this.ExecuteMethod = execute_Method;
            this.CanExecuteMethod = canexecute_Method;
        }

        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// ICommand Interface CanExecute Method 구현
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            return CanExecuteMethod(parameter);
        }
        /// <summary>
        /// ICommand Interface Execute Method 구현.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            ExecuteMethod(parameter);
        }
    }
}
