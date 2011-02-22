using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workflow.Model
{
    public interface IServiceLocator
    {
        TService GetService<TService>();
    }

    public interface IHumanResourcePolicyProvider
    {
        int GetRequiredDaysBetweenRaises( string department );
    }

    public class StandardResourcePolicyProvider : IHumanResourcePolicyProvider
    {
        /// department doesn't matter, DO AS CATBERT COMMANDS!!!
        public int GetRequiredDaysBetweenRaises(string department)
        {   
            return 30;
        }
    }

    public class Employee
    {
        protected IServiceLocator ServiceLocator { get; set; }

        public string EmployeeNumber { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public DateTime HireDate { get; protected set; }
        public string Department { get; protected set; }
        public bool IsNewHire { get; protected set; }
        public bool HasTakenOrientation { get; protected set; }
        public bool HasSignedForms { get; protected set; }
        public decimal PayRate { get; protected set; }
        public DateTime DateOfLastRaise { get; protected set; }


        


        public bool IsEligibleForRaise()
        {
            var hrPolicyProvider = ServiceLocator.GetService<IHumanResourcePolicyProvider>();
            var requiredDaysBetweenRaises = hrPolicyProvider.GetRequiredDaysBetweenRaises(Department);
            var effectiveLastRaiseDate = DateOfLastRaise == DateTime.MinValue ? HireDate : DateOfLastRaise;
            var daysSinceLastRaise = (DateTime.Now - effectiveLastRaiseDate).TotalDays;
            return daysSinceLastRaise >= requiredDaysBetweenRaises;
        }

        public Employee(IServiceLocator serviceLocator, 
            string employeeNumber, 
            string department, 
            string firstName, 
            string lastName, 
            DateTime hireDate)
        {
            ServiceLocator = serviceLocator;
            EmployeeNumber = employeeNumber;
            Department = department;
            FirstName = firstName;
            LastName = lastName;
            HireDate = hireDate;
        }
    }

    public class EmployeeFactory
    {
        protected IServiceLocator ServiceLocator { get; set; }

        public Employee CreateNewEmployee(string employeeNumber, string department, string firstName, string lastName, DateTime hireDate)
        {
            return new Employee(ServiceLocator, employeeNumber, department, firstName, lastName, hireDate);
        }

        public EmployeeFactory(IServiceLocator serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }
    }
}
