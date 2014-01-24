using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.Controllers;
using TechiesWeb.TeamBins.ViewModels;
using System.Linq;
namespace TeamBins.Tests
{

    [TestFixture]
    public class AccountControllerTest
    {
        [Test]
        public void Login_Action_Should_Return_Login_View()
        {
            //Arrange
            AccountController accntCntrl = new AccountController();
            
            //Act
            var result = accntCntrl.Login() as ViewResult;
            
            //Assert
            Assert.AreEqual(result.ViewName, "Login");
        }

        [Test]
        public void Login_Action_Should_Return_LoginVM_Object()
        {
            //Arrange
            AccountController accntCntrl = new AccountController();
            
            //Act
            var result = accntCntrl.Login() as ViewResult;
            var model=result.Model as LoginVM;
            
            //Assert
            Assert.IsNotNull(model);
        }

        [Test]
        public void ForgotPassword_Action_Should_Return_forgotPassword_View()
        {
            //Arrange
            AccountController accntCntrl = new AccountController();

            //Act
            var result = accntCntrl.forgotPassword() as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewName, "forgotPassword");
        }

        [Test]
        public void ForgotPassword_Action_Should_Return_ForgotPasswordVM_Object()
        {
            //Arrange
            AccountController accntCntrl = new AccountController();

            //Act
            var result = accntCntrl.forgotPassword() as ViewResult;
            var model = result.Model as ForgotPasswordVM;

            //Assert
            Assert.IsNotNull(model);
        }

    }
}
