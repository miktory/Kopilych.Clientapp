using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application
{
    internal class ApiEndpoints
    {
        private string _getUser;
        private string _status;
        private string _createUser;
        private string _deleteUser;
        private string _getCurrentUser;
        private string _login;
        private string _refreshSession;
        private string _logout;
        private string _getUserPiggyBankLinksByUserId;
        private string _createPiggyBank;
        private string _updatePiggyBank;
        private string _deletePiggyBank;
        private string _getPiggyBank;
        private string _getUserPiggyBank;
        private string _getPiggyBankCustomization;
        private string _getPiggyBankCustomizationByPiggyBankId;
        private string _updateUserPiggyBank;
        private string _updatePiggyBankCustomization;
        private string _createPiggyBankCustomization;
        private string _createUserPiggyBank;
        private string _getUserFriendshipsByUserId;
        private string _createUserFriendship;
        private string _deleteUserFriendship;
        private string _updateUserFriendship;
        private string _getUserFriendship;
        private string _getTransactionsByPiggyBankId;
        private string _createTransaction;
        private string _deleteTransaction;
        private string _updateTransaction;
        private string _getUserPiggyBankLinksByPiggyBankId;
        private string _deleteUserPiggyBank;
        private string _getUserPhoto;
        private string _updateUserPhoto;
        private string _deleteUserPhoto;
        private string _getPiggyBankCustomizationPhoto;
        private string _updatePiggyBankCustomizationPhoto;
        private string _deletePiggyBankCustomizationPhoto;

        public string ApiAddress { get; set; }

        public string GetCurrentUser { get => ApiAddress + _getCurrentUser; set => _getCurrentUser = value; }
        public string GetUser { get => ApiAddress + _getUser; set => _getUser = value; }
        public string CreateUser { get => ApiAddress + _createUser; set => _createUser = value; }
        public string UpdateUser { get => ApiAddress + _createUser; set => _createUser = value; }
        public string DeleteUser { get => ApiAddress + _deleteUser; set => _deleteUser = value; }
        public string GetUserPhoto { get => ApiAddress + _getUserPhoto; set => _getUserPhoto = value; }
        public string UpdateUserPhoto { get => ApiAddress + _updateUserPhoto; set => _updateUserPhoto = value; }
        public string DeleteUserPhoto { get => ApiAddress + _deleteUserPhoto; set => _deleteUserPhoto = value; }

        public string GetUserPiggyBankLinksByUserId { get => ApiAddress + _getUserPiggyBankLinksByUserId; set => _getUserPiggyBankLinksByUserId = value; }
        public string GetUserPiggyBankLinksByPiggyBankId { get => ApiAddress + _getUserPiggyBankLinksByPiggyBankId; set => _getUserPiggyBankLinksByPiggyBankId = value; }
        public string GetUserPiggyBank { get => ApiAddress + _getUserPiggyBank; set => _getUserPiggyBank = value; }
        public string UpdateUserPiggyBank { get => ApiAddress + _updateUserPiggyBank; set => _updateUserPiggyBank = value; }
        public string CreateUserPiggyBank { get => ApiAddress + _createUserPiggyBank; set => _createUserPiggyBank = value; }
        public string DeleteUserPiggyBank { get => ApiAddress + _deleteUserPiggyBank; set => _deleteUserPiggyBank = value; }

        public string CreatePiggyBank { get => ApiAddress + _createPiggyBank; set => _createPiggyBank = value; }
        public string UpdatePiggyBank { get => ApiAddress + _updatePiggyBank; set => _updatePiggyBank = value; }
        public string DeletePiggyBank { get => ApiAddress + _deletePiggyBank; set => _deletePiggyBank = value; }
        public string GetPiggyBank { get => ApiAddress + _getPiggyBank; set => _getPiggyBank = value; }

        public string GetPiggyBankCustomization { get => ApiAddress + _getPiggyBankCustomization; set => _getPiggyBankCustomization = value; }
        public string GetPiggyBankCustomizationByPiggyBankId { get => ApiAddress + _getPiggyBankCustomizationByPiggyBankId; set => _getPiggyBankCustomizationByPiggyBankId = value; }
        public string CreatePiggyBankCustomization { get => ApiAddress + _createPiggyBankCustomization; set => _createPiggyBankCustomization = value; }
        public string UpdatePiggyBankCustomization { get => ApiAddress + _updatePiggyBankCustomization; set => _updatePiggyBankCustomization = value; }
        public string GetPiggyBankCustomizationPhoto { get => ApiAddress + _getPiggyBankCustomizationPhoto; set => _getPiggyBankCustomizationPhoto = value; }
        public string UpdatePiggyBankCustomizationPhoto { get => ApiAddress + _updatePiggyBankCustomizationPhoto; set => _updatePiggyBankCustomizationPhoto = value; }
        public string DeletePiggyBankCustomizationPhoto { get => ApiAddress + _deletePiggyBankCustomizationPhoto; set => _deletePiggyBankCustomizationPhoto = value; }

        public string RefreshSession { get => ApiAddress + _refreshSession; set => _refreshSession = value; }
        public string Login { get => ApiAddress + _login; set => _login = value; }
        public string Logout { get => ApiAddress + _logout; set => _logout = value; }

        public string GetUserFriendshipsByUserId { get => ApiAddress + _getUserFriendshipsByUserId; set => _getUserFriendshipsByUserId = value; }
        public string CreateUserFriendship { get => ApiAddress + _createUserFriendship; set => _createUserFriendship = value; }
        public string DeleteUserFriendship { get => ApiAddress + _deleteUserFriendship; set => _deleteUserFriendship = value; }
        public string UpdateUserFriendship { get => ApiAddress + _updateUserFriendship; set => _updateUserFriendship = value; }
        public string GetUserFriendship { get => ApiAddress + _getUserFriendship; set => _getUserFriendship = value; }

        public string GetTransactionsByPiggyBankId { get => ApiAddress + _getTransactionsByPiggyBankId; set => _getTransactionsByPiggyBankId = value; }
        public string CreateTransaction { get => ApiAddress + _createTransaction; set => _createTransaction = value; }
        public string UpdateTransaction { get => ApiAddress + _updateTransaction; set => _updateTransaction = value; }
        public string DeleteTransaction { get => ApiAddress + _deleteTransaction; set => _deleteTransaction = value; }

        public string HealthcheckAddress { get => _status; set => _status = value; }

    }

    public static class ApiStringExtensions
    {
        public static string ReplacePlaceholders(this string template, Dictionary<string, string> values)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (var kvp in values)
            {
                // Замена "{ключ}" на значение из словаря
                template = template.Replace("{" + kvp.Key + "}", kvp.Value);
            }

            return template;
        }
    }
}
