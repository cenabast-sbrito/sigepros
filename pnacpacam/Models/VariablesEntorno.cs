using System;
using System.ComponentModel.DataAnnotations;

namespace pnacpacam.Models
{
    public class VariablesEntorno
    {
        [DataType(DataType.Date)]
        private DateTime _nowDate = DateTime.Today;
        [DataType(DataType.Date)]
        private DateTime _firstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        [DataType(DataType.Date)]
        private DateTime _lastDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
        [DataType(DataType.Date)]
        private DateTime _lastCloseMonth = DateTime.Today;
        [DataType(DataType.Date)]
        private DateTime _monthClosingDate = DateTime.Today;
        [DataType(DataType.Date)]
        private DateTime _firstDateLastCloseMonth;
        [DataType(DataType.Date)]
        private DateTime _lastDateLastCloseMonth;
        [DataType(DataType.Date)]
        private DateTime _firstDateNewCloseMonth;
        [DataType(DataType.Date)]
        private DateTime _lastDateNewCloseMonth;
        public VariablesEntorno()
        {
            _firstDate = DateTime.Today;

        }
        public DateTime firstDate
        {
            get { return _firstDate; }
            set { _firstDate = DateTime.Today; }
        }
        public DateTime lastDate
        {
            get { return _lastDate; }
            set { _lastDate = value; }
        }
        public DateTime nowDate
        {
            get { return _nowDate; }
            set { _nowDate = new DateTime(); }
        }
    }
}