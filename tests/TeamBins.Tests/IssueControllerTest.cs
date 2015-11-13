//using NUnit.Framework;
//using Rhino.Mocks;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;
//using TeamBins.DataAccess;
//using TechiesWeb.TeamBins.Controllers;
//using TechiesWeb.TeamBins.ViewModels;

//namespace TeamBins.Tests
//{
//    [TestFixture]
//    public class IssueControllerTest
//    {
//        [Test]
//        public void Index_Action_Should_Return_Index_View()
//        {
        

//            //Arrange
//            FakeDataStore fakeDb = new FakeDataStore();

//            FakeRepositary repo = new FakeRepositary(fakeDb);

//            IssuesController issueCtrl = new IssuesController(repo);

//            //Act
//            var result = issueCtrl.Index() as ViewResult;

//            //Assert
//            Assert.AreEqual(result.ViewName, "Index");
//        }

//        [Test]
//        public void Index_Action_Should_Return_BugsListVM_Object_To_View()
//        {
//            //Arrange
//            FakeDataStore fakeDb = new FakeDataStore();

//            FakeRepositary repo = new FakeRepositary(fakeDb);

//            IssuesController issueCtrl = new IssuesController(repo);

//            //Act
//            var result = issueCtrl.Index() as ViewResult;

//            var resultModel = result.Model as IssueListVM;
//            //Assert
//            Assert.IsNotNull(resultModel);
//        }

//        [Test]
//        public void Index_Action_Should_Return_Error_View_If_There_Is_Any_Error()
//        {
//            //Arrange
//            var repository = MockRepository.GenerateStrictMock<IRepositary>();
//            repository.Stub(s => s.GetProjects(Arg<int>.Is.Anything)).Throw(new Exception());
//            IssuesController issueCtrl = new IssuesController(repository);
            
//            //Act
//            var result = issueCtrl.Index() as ViewResult;          

//            //Assert
//            Assert.AreEqual(result.ViewName, "Error");
//        }

//        [Test]
//        public void Index_Action_Should_Return_BugsListVM_Object_With_Correct_Number_Of_Issues()
//        {
//            //IF the db has 2 issues of Location "SPRNT", the Index action should return 2 items in the Model/ViewModel

//            //Arrange
            
//            const int fakeTeamID = 1;

//            var _session = MockRepository.GenerateStrictMock<HttpSessionStateBase>();
//            _session.Stub(s => s["TB_TeamID"]).Return(fakeTeamID);
//            _session.Stub(s => s["CreateAndEditMode"]).Return(false);

//            var _context = MockRepository.GenerateStrictMock<HttpContextBase>();
//            _context.Stub(c => c.Session).Return(_session);

//            var projectList = new List<Project>();
//            projectList.Add(new Project { ID = 1, Name = "TeamBinPro", TeamID = fakeTeamID });
//            projectList.Add(new Project { ID = 2, Name = "kTable", TeamID = fakeTeamID });
           
//            var issueList = new List<Issue>();
//            issueList.Add(new Issue { ID = 1, Title = "js error", Description = "test", TeamID = fakeTeamID, Location = "SPRNT", CreatedBy = new User { ID = 1, FirstName = "Scott" }, Priority = new Priority { ID = 1, Name = "High" }, Category = new Category { ID = 1, Name = "Bug" }, Status = new Status { ID = 1, Name = "New" }, Project = projectList[0] });
//            issueList.Add(new Issue { ID = 2, Title = "db error", Description = "test", TeamID = fakeTeamID, Location = "SPRNT", CreatedBy = new User { ID = 1, FirstName = "Scott" }, Priority = new Priority { ID = 1, Name = "High" }, Category = new Category { ID = 1, Name = "Bug" }, Status = new Status { ID = 1, Name = "New" }, Project = projectList[0] });
//            issueList.Add(new Issue { ID = 3, Title = "error", Description = "test", TeamID = fakeTeamID, Location = "BKLOG", CreatedBy = new User { ID = 1, FirstName = "Scott" }, Priority = new Priority { ID = 1, Name = "High" }, Category = new Category { ID = 1, Name = "Bug" }, Status = new Status { ID = 1, Name = "New" }, Project = projectList[0] });

//            var repository = MockRepository.GenerateStrictMock<IRepositary>();

//            repository.Stub(s => s.GetProjects(Arg<int>.Is.Anything)).Return(projectList);
//            repository.Stub(s => s.GetIssues()).Return(issueList);

//            IssuesController issueCtrl = new IssuesController(repository);
//            issueCtrl.ControllerContext = new ControllerContext(_context, new RouteData(), issueCtrl);
            
//            //Act
//            var result = issueCtrl.Index() as ViewResult;
//            var resultModel = result.Model as IssueListVM;

//            //Assert
//            Assert.AreEqual(resultModel.Bugs.Count, 2);
//        }


//    }
//}
