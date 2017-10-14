namespace CRMSecurityProvider.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web.Security;

    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    using CRMSecurityProvider.Caching;
    using CRMSecurityProvider.Configuration;
    using CRMSecurityProvider.Repository;
    using CRMSecurityProvider.Repository.Factory;
    using Microsoft.Xrm.Sdk.Query;
    /// <summary>
    /// The CrmMembershipProvider test class.
    /// </summary>
    public class CrmMembershipProviderTest
    {
        private readonly UserRepositoryBase userRepository;
        private readonly ProfileRepositoryBase profileRepository;

        private readonly IUserRepositoryFactory userRepositoryFactory;
        private readonly IProfileRepositoryFactory profileRepositoryFactory;

        private readonly CRMMembershipProvider crmMembershipProvider;
        private readonly CRMUser user;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmMembershipProviderTest"/> class.
        /// </summary>
        public CrmMembershipProviderTest()
        {
            var cacheService = Substitute.For<ICacheService>();

            this.userRepository = Substitute.For<UserRepositoryBase>(cacheService);
            this.profileRepository = Substitute.For<ProfileRepositoryBase>(this.userRepository, cacheService);

            this.userRepositoryFactory = Substitute.For<IUserRepositoryFactory>();
            this.userRepositoryFactory.GetRepository(Arg.Any<ConfigurationSettings>()).Returns(this.userRepository);

            this.profileRepositoryFactory = Substitute.For<IProfileRepositoryFactory>();
            this.profileRepositoryFactory.GetRepository(Arg.Any<ConfigurationSettings>()).Returns(this.profileRepository);

            var config = new NameValueCollection();
            config.Add("name", "crm");
            config.Add("applicationName", "CRM security provider");
            config.Add("readOnly", "false");
            config.Add("connectionStringName", "CRMConnString");

            this.crmMembershipProvider = new CRMMembershipProvider(this.userRepositoryFactory, this.profileRepositoryFactory);
            this.crmMembershipProvider.Initialize(config["name"], config);

            this.user = new CRMUser(
              "test@sitecore.net",
              Guid.NewGuid(),
              "test@sitecore.net",
              null,
              String.Empty,
              true,
              false,
              DateTime.Now,
              DateTime.Now,
              DateTime.MinValue,
              DateTime.MinValue,
              DateTime.MinValue);
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldCreateUser()
        {
            // Arrange
            MembershipCreateStatus status;
            this.userRepository.CreateUser(this.user.Name, this.user.Email, Guid.Empty).Returns(this.user);

            // Act
            var result = this.crmMembershipProvider.CreateUser(this.user.Name, "test", this.user.Email, null, null, true, Guid.Empty, out status);

            // Assert
            result.Should().NotBeNull();
            result.UserName.ShouldBeEquivalentTo(this.user.Name);
            result.Email.ShouldBeEquivalentTo(this.user.Email);
            result.ProviderUserKey.ShouldBeEquivalentTo(this.user.ID);
            status.ShouldBeEquivalentTo(MembershipCreateStatus.Success);
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldCreateUserWithSpecifiedId()
        {
            // Arrange
            MembershipCreateStatus status;
            this.userRepository.CreateUser(this.user.Name, this.user.Email, this.user.ID).Returns(this.user);

            // Act
            var result = this.crmMembershipProvider.CreateUser(this.user.Name, "test", this.user.Email, null, null, true, this.user.ID, out status);

            // Assert
            result.Should().NotBeNull();
            result.UserName.ShouldBeEquivalentTo(this.user.Name);
            result.Email.ShouldBeEquivalentTo(this.user.Email);
            result.ProviderUserKey.ShouldBeEquivalentTo(this.user.ID);
            status.ShouldBeEquivalentTo(MembershipCreateStatus.Success);
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldNotCreateUserWithDuplicatedUserName()
        {
            // Arrange
            MembershipCreateStatus status;
            this.userRepository.GetUser(this.user.Name).Returns(this.user);

            // Act
            var result = this.crmMembershipProvider.CreateUser(this.user.Name, "test", this.user.Email, null, null, true, Guid.Empty, out status);

            // Assert
            result.Should().BeNull();
            status.ShouldBeEquivalentTo(MembershipCreateStatus.DuplicateUserName);
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldNotCreateUserWithInvalidProviderUserKey()
        {
            // Arrange
            MembershipCreateStatus status;

            // Act
            var result = this.crmMembershipProvider.CreateUser(this.user.Name, "test", this.user.Email, null, null, true, new object(), out status);

            // Assert
            result.Should().BeNull();
            status.ShouldBeEquivalentTo(MembershipCreateStatus.InvalidProviderUserKey);
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldNotCreateUserWithDuplicatedProviderUserKey()
        {
            // Arrange
            MembershipCreateStatus status;
            this.userRepository.GetUser(this.user.ID).Returns(this.user);

            // Act
            var result = this.crmMembershipProvider.CreateUser(this.user.Name, "test", this.user.Email, null, null, true, this.user.ID, out status);

            // Assert
            result.Should().BeNull();
            status.ShouldBeEquivalentTo(MembershipCreateStatus.DuplicateProviderUserKey);
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldDeleteUser()
        {
            // Arrange
            this.userRepository.DeactivateUser(this.user.Name).Returns(true);

            // Act
            var result = this.crmMembershipProvider.DeleteUser(this.user.Name, true);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldNotDeleteMissingUser()
        {
            // Arrange
            this.userRepository.GetUser(this.user.Name).Returns((CRMUser)null);

            // Act
            var result = this.crmMembershipProvider.DeleteUser(this.user.Name, true);

            // Assert
            result.Should().BeFalse();
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldFindUserByEmail()
        {
            // Arrange
            int totalRecords;
            this.userRepository.FindUsersByEmail(this.user.Email, 0, 10, out totalRecords).Returns(new List<CRMUser> { this.user });

            // Act
            var result = this.crmMembershipProvider.FindUsersByEmail(this.user.Email, 0, 10, out totalRecords);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[this.user.Name].UserName.ShouldBeEquivalentTo(this.user.Name);
            result[this.user.Name].Email.ShouldBeEquivalentTo(this.user.Email);
            result[this.user.Name].ProviderUserKey.ShouldBeEquivalentTo(this.user.ID);
        }

        [Fact]
        public void ShouldReturnEmptyCollectionIfUserWithSpecifiedEmailIsMissing()
        {
            // Arrange
            int totalRecords;
            this.userRepository.FindUsersByEmail(this.user.Email, 0, 10, out totalRecords).Returns(new List<CRMUser>());

            // Act
            var result = this.crmMembershipProvider.FindUsersByEmail(this.user.Email, 0, 10, out totalRecords);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldFindUserByName()
        {
            // Arrange
            int totalRecords;
            this.userRepository.FindUsersByName(this.user.Name, 0, 10, out totalRecords).Returns(new List<CRMUser> { this.user });

            // Act
            var result = this.crmMembershipProvider.FindUsersByName(this.user.Name, 0, 10, out totalRecords);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[this.user.Name].UserName.ShouldBeEquivalentTo(this.user.Name);
            result[this.user.Name].Email.ShouldBeEquivalentTo(this.user.Email);
            result[this.user.Name].ProviderUserKey.ShouldBeEquivalentTo(this.user.ID);
        }

        [Fact]
        public void ShouldReturnEmptyCollectionIfUserWithSpecifiedNameIsMissing()
        {
            // Arrange
            int totalRecords;
            this.userRepository.FindUsersByName(this.user.Name, 0, 10, out totalRecords).Returns(new List<CRMUser>());

            // Act
            var result = this.crmMembershipProvider.FindUsersByName(this.user.Name, 0, 10, out totalRecords);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldGetAllUsers()
        {
            // Arrange
            int totalRecords;
            var pagingInfo = new PagingInfo { Count = 10 };
            this.userRepository.GetAllUsers(pagingInfo, out totalRecords).ReturnsForAnyArgs(new List<CRMUser> { this.user });

            // Act
            var result = this.crmMembershipProvider.GetAllUsers(0, 10, out totalRecords);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[this.user.Name].UserName.ShouldBeEquivalentTo(this.user.Name);
            result[this.user.Name].Email.ShouldBeEquivalentTo(this.user.Email);
            result[this.user.Name].ProviderUserKey.ShouldBeEquivalentTo(this.user.ID);
        }

        [Fact]
        public void ShouldReturnEmptyCollectionIfThereAreNoUsersInCrm()
        {
            // Arrange
            int totalRecords;
            var pagingInfo = new PagingInfo { Count = 10 };
            this.userRepository.GetAllUsers(pagingInfo, out totalRecords).ReturnsForAnyArgs(new List<CRMUser>());

            // Act
            var result = this.crmMembershipProvider.GetAllUsers(0, 10, out totalRecords);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldGetUserByName()
        {
            // Arrange
            this.userRepository.GetUser(this.user.Name).Returns(this.user);

            // Act
            var result = this.crmMembershipProvider.GetUser(this.user.Name, true);

            // Assert
            result.Should().NotBeNull();
            result.UserName.ShouldBeEquivalentTo(this.user.Name);
            result.Email.ShouldBeEquivalentTo(this.user.Email);
            result.ProviderUserKey.ShouldBeEquivalentTo(this.user.ID);
        }

        [Fact]
        public void ShouldReturnNullIfUserWithSpecifiedNameIsMissing()
        {
            // Arrange
            this.userRepository.GetUser(this.user.Name).Returns((CRMUser)null);

            // Act
            var result = this.crmMembershipProvider.GetUser(this.user.Name, true);

            // Assert
            result.Should().BeNull();
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldGetUserById()
        {
            // Arrange
            this.userRepository.GetUser(this.user.ID).Returns(this.user);

            // Act
            var result = this.crmMembershipProvider.GetUser(this.user.ID, true);

            // Assert
            result.Should().NotBeNull();
            result.UserName.ShouldBeEquivalentTo(this.user.Name);
            result.Email.ShouldBeEquivalentTo(this.user.Email);
            result.ProviderUserKey.ShouldBeEquivalentTo(this.user.ID);
        }

        [Fact]
        public void ShouldReturnNullIfUserWithSpecifiedIdIsMissing()
        {
            // Arrange
            this.userRepository.GetUser(this.user.ID).Returns((CRMUser)null);

            // Act
            var result = this.crmMembershipProvider.GetUser(this.user.ID, true);

            // Assert
            result.Should().BeNull();
        }

        [Fact(Skip = "Requires Dynamics instance.")]
        public void ShouldGetUserNameByEmail()
        {
            // Arrange
            int totalRecords;
            this.userRepository.FindUsersByEmail(this.user.Email, 0, 1, out totalRecords).Returns(new List<CRMUser> { this.user });

            // Act
            var result = this.crmMembershipProvider.GetUserNameByEmail(this.user.Email);

            // Assert
            result.ShouldBeEquivalentTo(this.user.Name);
        }

        [Fact]
        public void ShouldReturnNullAsEmailIfUserWithSpecifiedNameIsMissing()
        {
            // Arrange
            int totalRecords;
            this.userRepository.FindUsersByEmail(this.user.Email, 0, 1, out totalRecords).Returns(new List<CRMUser>());

            // Act
            var result = this.crmMembershipProvider.GetUserNameByEmail(this.user.Email);

            // Assert
            result.Should().BeNull();
        }
    }
}
