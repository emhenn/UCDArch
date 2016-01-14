using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.RegressionTests.SampleMappings;
using UCDArch.Testing;
using Xunit;

namespace UCDArch.RegressionTests.Repository
{
    public class DomainObjectRetrievalTests : FluentRepositoryTestBase<UserMap>
    {
        private readonly IRepository<User> _userRepository = new Repository<User>();
        protected override void LoadData()
        {
            DomainObjectDataHelper.LoadDomainDataUsers(_userRepository);
        }

        /// <summary>
        /// Determines whether this instance [can get existing user].
        /// </summary>
        [Fact]
        public void CanGetExistingUser()
        {
            var user = _userRepository.GetNullableById(2);
            Assert.NotNull(user);
            Assert.Equal("aaaaa", user.LoginID);
            Assert.Equal("John", user.FirstName);
            Assert.Equal(2, user.Id);
        }

        /// <summary>
        /// Gets the user that does not exist returns proxy.
        /// </summary>
        [Fact]
        public void GetUserThatDoesNotExistReturnsNull()
        {
            var user = _userRepository.GetNullableById(99); //99 does not exist
            Assert.Null(user);
        }

        /// <summary>
        /// Determines whether this instance [can get existing user with get nullable].
        /// </summary>
        [Fact]
        public void CanGetExistingUserWithGetNullable()
        {
            var user = _userRepository.GetNullableById(2);
            Assert.NotNull(user);
            Assert.Equal("aaaaa", user.LoginID);
            Assert.Equal("John", user.FirstName);
            Assert.Equal(2, user.Id);
        }

        /// <summary>
        /// Gets the user with get nullable that does not exist returns null.
        /// </summary>
        [Fact]
        public void GetUserWithGetNullableThatDoesNotExistReturnsNull()
        {
            var user = _userRepository.GetNullableById(99); //99 does not exist
            Assert.Null(user);
        }

        /// <summary>
        /// Gets all users returns all users.
        /// </summary>
        [Fact]
        public void GetAllUsersReturnsAllUsers()
        {
            var users = _userRepository.GetAll();
            Assert.NotNull(users);
            Assert.Equal(10, users.Count);
            Assert.Equal("John", users[1].FirstName);
            Assert.Equal("aaaaa", users[1].LoginID);
            Assert.Equal(2, users[1].Id);
        }

        /// <summary>
        /// Gets all users sort by login ascending returns correct order.
        /// </summary>
        [Fact]
        public void GetAllUsersSortByLoginAscendingReturnsCorrectOrder()
        {
            var orderedUsers = new List<OrderedUsers>
                                   {
                                       new OrderedUsers {Name = "John", Login = "aaaaa", Id = 2},
                                       new OrderedUsers {Name = "James", Login = "bbbbb", Id = 3},
                                       new OrderedUsers {Name = "Bob", Login = "ccccc", Id = 4},
                                       new OrderedUsers {Name = "Larry", Login = "ddddd", Id = 5},
                                       new OrderedUsers {Name = "Joe", Login = "eeeee", Id = 6},
                                       new OrderedUsers {Name = "Pete", Login = "fffff", Id = 7},
                                       new OrderedUsers {Name = "Adam", Login = "ggggg", Id = 8},
                                       new OrderedUsers {Name = "Alan", Login = "hhhhh", Id = 9},
                                       new OrderedUsers {Name = "Ken", Login = "iiiii", Id = 10},
                                       new OrderedUsers {Name = "Scott", Login = "postit", Id = 1}
                                   };

            var users = _userRepository.GetAll("LoginID", true);
            Assert.NotNull(users);
            Assert.Equal(10, users.Count);

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(orderedUsers[i].Name, users[i].FirstName);
                Assert.Equal(orderedUsers[i].Login, users[i].LoginID);
                Assert.Equal(orderedUsers[i].Id, users[i].Id);
            }
        }


        /// <summary>
        /// Gets all users sort by login decending returns correct order.
        /// </summary>
        [Fact]
        public void GetAllUsersSortByLoginDecendingReturnsCorrectOrder()
        {
            var orderedUsers = new List<OrderedUsers>
                                   {
                                       new OrderedUsers {Name = "Scott", Login = "postit", Id = 1},
                                       new OrderedUsers {Name = "Ken", Login = "iiiii", Id = 10},
                                       new OrderedUsers {Name = "Alan", Login = "hhhhh", Id = 9},
                                       new OrderedUsers {Name = "Adam", Login = "ggggg", Id = 8},
                                       new OrderedUsers {Name = "Pete", Login = "fffff", Id = 7},
                                       new OrderedUsers {Name = "Joe", Login = "eeeee", Id = 6},
                                       new OrderedUsers {Name = "Larry", Login = "ddddd", Id = 5},
                                       new OrderedUsers {Name = "Bob", Login = "ccccc", Id = 4},
                                       new OrderedUsers {Name = "James", Login = "bbbbb", Id = 3},
                                       new OrderedUsers {Name = "John", Login = "aaaaa", Id = 2}
                                   };


            var users = _userRepository.GetAll("LoginID", false);
            Assert.NotNull(users);
            Assert.Equal(10, users.Count);

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(orderedUsers[i].Name, users[i].FirstName);
                Assert.Equal(orderedUsers[i].Login, users[i].LoginID);
                Assert.Equal(orderedUsers[i].Id, users[i].Id);
            }
        }

        /// <summary>
        /// Gets all users sort by first name ascending returns correct order.
        /// </summary>
        [Fact]
        public void GetAllUsersSortByFirstNameAscendingReturnsCorrectOrder()
        {
            var orderedUsers = new List<OrderedUsers>
                                   {
                                       new OrderedUsers {Name = "Adam", Login = "ggggg", Id = 8},
                                       new OrderedUsers {Name = "Alan", Login = "hhhhh", Id = 9},
                                       new OrderedUsers {Name = "Bob", Login = "ccccc", Id = 4},
                                       new OrderedUsers {Name = "James", Login = "bbbbb", Id = 3},
                                       new OrderedUsers {Name = "Joe", Login = "eeeee", Id = 6},
                                       new OrderedUsers {Name = "John", Login = "aaaaa", Id = 2},
                                       new OrderedUsers {Name = "Ken", Login = "iiiii", Id = 10},
                                       new OrderedUsers {Name = "Larry", Login = "ddddd", Id = 5},
                                       new OrderedUsers {Name = "Pete", Login = "fffff", Id = 7},
                                       new OrderedUsers {Name = "Scott", Login = "postit", Id = 1}
                                   };

            var users = _userRepository.GetAll("FirstName", true);
            Assert.NotNull(users);
            Assert.Equal(10, users.Count);

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(orderedUsers[i].Name, users[i].FirstName);
                Assert.Equal(orderedUsers[i].Login, users[i].LoginID);
                Assert.Equal(orderedUsers[i].Id, users[i].Id);
            }
        }

        /// <summary>
        /// Wrongs the property name throws exception.
        /// </summary>
        [Fact]
        public void WrongPropertyNameThrowsException()
        {
            try
            {
                IList<User> users = _userRepository.GetAll("bbb", true);
                Assert.True(false, "Expected QueryException");
            }
            catch (QueryException message)
            {
                Assert.Equal("could not resolve property: bbb of: UCDArch.RegressionTests.SampleMappings.User", message.Message );
            }
        }

        /// <summary>
        /// Queryable the first name is larry returns expected result.
        /// </summary>
        [Fact]
        public void QueryableWhereFirstNameIsLarryReturnsExpectedResult()
        {
            IQueryable<User> users = _userRepository.Queryable.Where(u => u.FirstName == "Larry");

            var usersList = users.ToList();
            Assert.NotNull(usersList);
            Assert.Equal(1, usersList.Count);
            Assert.Equal("ddddd", usersList[0].LoginID);
        }

        /// <summary>
        /// Queryables the where first name starts with J returns expected results.
        /// </summary>
        [Fact]
        public void QueryableWhereFirstNameStartsWithJReturnsExpectedResults()
        {
            var users = _userRepository.Queryable.Where(u => u.FirstName.StartsWith("J")).OrderBy(u => u.FirstName) .ToList();
            var names = new List<string>{"James", "Joe", "John"};

            Assert.NotNull(users);
            Assert.Equal(3, users.Count);
            foreach (var user in users)
            {
                Assert.True(names.Contains(user.FirstName));
            }
        }

        /// <summary>
        /// Easier to reorder the ordered fields that are checked.
        /// </summary>
        public struct OrderedUsers
        {
            public string Name;
            public string Login;
            public int Id;
        }
    }
}
