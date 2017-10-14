namespace CRMSecurityProvider.Test
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;

  using FluentAssertions;
  using NSubstitute;
  using Xunit;

  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.Repository;
  using CRMSecurityProvider.Repository.Factory;

  /// <summary>
  /// The CrmRoleProvider test class.
  /// </summary>
  public class CrmRoleProviderTest
  {
    private readonly RoleRepositoryBase roleRepository;
    private readonly IRoleRepositoryFactory roleRepositoryFactory;
    private readonly CRMRoleProvider crmRoleProvider;
    private readonly CRMRole role;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmRoleProviderTest"/> class.
    /// </summary>
    public CrmRoleProviderTest()
    {
      var cacheService = Substitute.For<ICacheService>();
      var userRepository = Substitute.For<UserRepositoryBase>(cacheService);

      this.roleRepository = Substitute.For<RoleRepositoryBase>(userRepository, cacheService);

      this.roleRepositoryFactory = Substitute.For<IRoleRepositoryFactory>();
      this.roleRepositoryFactory.GetRepository(Arg.Any<ConfigurationSettings>()).Returns(this.roleRepository);

      var config = new NameValueCollection();
      config.Add("name", "crm");
      config.Add("applicationName", "CRM security provider");
      config.Add("readOnly", "false");
      config.Add("connectionStringName", "CRMConnString");

      this.crmRoleProvider = new CRMRoleProvider(this.roleRepositoryFactory);
      this.crmRoleProvider.Initialize(config["name"], config);

      this.role = new CRMRole("test", Guid.NewGuid());
    }

    [Fact]
    public void ShouldCreateRole()
    {
      // Arrange
      this.roleRepository.CreateRole(this.role.Name).Returns(true);

      // Act
      Action action = () => this.crmRoleProvider.CreateRole(this.role.Name);

      // Assert
      action.ShouldNotThrow<NotSupportedException>();
      action.ShouldNotThrow<ApplicationException>();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldNotCreateRoleWithDuplicatedRoleName()
    {
      // Arrange
      this.roleRepository.GetRole(this.role.Name).Returns(this.role);

      // Act
      Action action = () => this.crmRoleProvider.CreateRole(this.role.Name);

      // Assert
      action.ShouldThrow<NotSupportedException>();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldThrowExceptionIfItIsImpossibleToCreateRole()
    {
      // Arrange
      this.roleRepository.CreateRole(this.role.Name).Returns(false);

      // Act
      Action action = () => this.crmRoleProvider.CreateRole(this.role.Name);

      // Assert
      action.ShouldThrow<ApplicationException>();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldDeleteRole()
    {
      // Arrange
      this.roleRepository.DeactivateRole(this.role.Name).Returns(true);

      // Act
      var result = this.crmRoleProvider.DeleteRole(this.role.Name, false);

      // Assert
      result.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotDeleteMissingRole()
    {
      // Arrange
      this.roleRepository.GetRole(this.role.Name).Returns((CRMRole)null);

      // Act
      var result = this.crmRoleProvider.DeleteRole(this.role.Name, false);

      // Assert
      result.Should().BeFalse();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldReturnTrueForUserInRole()
    {
      // Arrange
      var userName = "test";
      this.roleRepository.IsUserInRole(userName, this.role.Name).Returns(true);

      // Act
      var result = this.crmRoleProvider.IsUserInRole(userName, this.role.Name);

      // Assert
      result.Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnFalseForUserOutOfRole()
    {
      // Arrange
      var userName = "test";
      this.roleRepository.IsUserInRole(userName, this.role.Name).Returns(false);

      // Act
      var result = this.crmRoleProvider.IsUserInRole(userName, this.role.Name);

      // Assert
      result.Should().BeFalse();
    }

    [Fact]
    public void ShouldAddUsersToRoles()
    {
      // Arrange
      var userNames = new string[] { "test" };
      var roleNames = new string[] { this.role.Name };
      this.roleRepository.AddUsersToRoles(userNames, roleNames).Returns(true);

      // Act
      Action action = ()=> this.crmRoleProvider.AddUsersToRoles(userNames, roleNames);

      // Assert
      action.ShouldNotThrow<NotSupportedException>();
      action.ShouldNotThrow<ApplicationException>();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldThrowExceptionIfItIsImpossibleToAddUsersToRoles()
    {
      // Arrange
      var userNames = new string[] { "test" };
      var roleNames = new string[] { this.role.Name };
      this.roleRepository.AddUsersToRoles(userNames, roleNames).Returns(false);

      // Act
      Action action = () => this.crmRoleProvider.AddUsersToRoles(userNames, roleNames);

      // Assert
      action.ShouldThrow<ApplicationException>();
    }

    [Fact]
    public void ShouldRemoveUsersFromRoles()
    {
      // Arrange
      var userNames = new string[] { "test" };
      var roleNames = new string[] { this.role.Name };
      this.roleRepository.RemoveUsersFromRoles(userNames, roleNames).Returns(true);

      // Act
      Action action = () => this.crmRoleProvider.RemoveUsersFromRoles(userNames, roleNames);

      // Assert
      action.ShouldNotThrow<NotSupportedException>();
      action.ShouldNotThrow<ApplicationException>();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldThrowExceptionIfItIsImpossibleToRemoveUsersFromRoles()
    {
      // Arrange
      var userNames = new string[] { "test" };
      var roleNames = new string[] { this.role.Name };
      this.roleRepository.RemoveUsersFromRoles(userNames, roleNames).Returns(false);

      // Act
      Action action = () => this.crmRoleProvider.RemoveUsersFromRoles(userNames, roleNames);

      // Assert
      action.ShouldThrow<ApplicationException>();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldGetAllRoles()
    {
      // Arrange
      this.roleRepository.GetAllRoles().Returns(new List<CRMRole> { this.role });

      // Act
      var result = this.crmRoleProvider.GetAllRoles();

      // Assert
      result.Should().NotBeNull();
      result.Length.Should().Be(1);
      result[0].ShouldBeEquivalentTo(this.role.Name);
    }

    [Fact]
    public void ShouldReturnEmptyArrayIfThereAreNoRolesInCrm()
    {
      // Arrange
      this.roleRepository.GetAllRoles().Returns(new List<CRMRole>());

      // Act
      var result = this.crmRoleProvider.GetAllRoles();

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldReturnTrueForExistingRole()
    {
      // Arrange
      this.roleRepository.GetRole(this.role.Name).Returns(this.role);

      // Act
      var result = this.crmRoleProvider.RoleExists(this.role.Name);

      // Assert
      result.Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnFalseForMissingRole()
    {
      // Arrange
      this.roleRepository.GetRole(this.role.Name).Returns((CRMRole)null);

      // Act
      var result = this.crmRoleProvider.RoleExists(this.role.Name);

      // Assert
      result.Should().BeFalse();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldGetRolesForUser()
    {
      // Arrange
      var userName = "test";
      this.roleRepository.GetRolesForUser(userName).Returns(new string[] { this.role.Name });

      // Act
      var result = this.crmRoleProvider.GetRolesForUser(userName);

      // Assert
      result.Should().NotBeNull();
      result.Length.Should().Be(1);
      result[0].ShouldBeEquivalentTo(this.role.Name);
    }

    [Fact]
    public void ShouldReturnEmptyArrayIfThereAreNoRolesForUser()
    {
      // Arrange
      var userName = "test";
      this.roleRepository.GetRolesForUser(userName).Returns(new string[0]);

      // Act
      var result = this.crmRoleProvider.GetRolesForUser(userName);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldGetUsersInRole()
    {
      // Arrange
      var userName = "test";
      this.roleRepository.GetUsersInRole(this.role.Name).Returns(new string[] { userName });

      // Act
      var result = this.crmRoleProvider.GetUsersInRole(this.role.Name);

      // Assert
      result.Should().NotBeNull();
      result.Length.Should().Be(1);
      result[0].ShouldBeEquivalentTo(userName);
    }

    [Fact]
    public void ShouldReturnEmptyArrayIfThereAreNoUsersInRole()
    {
      // Arrange
      this.roleRepository.GetUsersInRole(this.role.Name).Returns(new string[0]);

      // Act
      var result = this.crmRoleProvider.GetUsersInRole(this.role.Name);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }

    [Fact(Skip = "Requires Dynamics instance.")]
    public void ShouldFindUserInRole()
    {
      // Arrange
      var userName = "test";
      this.roleRepository.GetUsersInRole(this.role.Name).Returns(new string[] { userName });

      // Act
      var result = this.crmRoleProvider.FindUsersInRole(this.role.Name, userName);

      // Assert
      result.Should().NotBeNull();
      result.Length.Should().Be(1);
      result[0].ShouldBeEquivalentTo(userName);
    }

    [Fact]
    public void ShouldReturnEmptyArrayIfThereAreNoSpecifiedUserInRole()
    {
      // Arrange
      var userName = "test";
      this.roleRepository.GetUsersInRole(this.role.Name).Returns(new string[0]);

      // Act
      var result = this.crmRoleProvider.FindUsersInRole(this.role.Name, userName);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }
  }
}
