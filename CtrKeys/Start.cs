using System;
using System.Collections.Generic;
using System.Text;

namespace CtrKeys
{
    /// <summary>
    /// не важно как мы назвали класс, ну, типа отсель стартуем
    /// </summary>
    public class Start
    {
        StringBuilder messages = new StringBuilder();

        /// <summary>
        ///  метод Пыщ!
        /// </summary>
        /// <param name="selectMethod"></param>
        /// <returns></returns>
        public StringBuilder Pysch(int selectMethod)
        {
            // тут у нас одновременно и подписка на событие и добавление в накопитель сообщений, на которые мы подписались
            //Program.ReportHandler += (sender, args) => messagesList.Add(sender.ToString());
            // можно то же самое сделать проще, через метод :) но два раза это делать не надо
            Program.ReportHandler += OnReportHandler;
            

            Program.Main(new[] {selectMethod.ToString()});

            return messages;
        }

        private void OnReportHandler(object sender, EventArgs args)
        {
            messages.AppendLine(sender.ToString());
        }
    }
}
