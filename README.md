MVCWhenThenFramework
====================

This project expands on others that are out there by adding support for HttpContext.Current mocking. 

SAMPLE USAGE:

		// useHttpContextConstructor: false means mock everything
		// useHttpContextConstructor: true means using reflection actually instantiate 
		// HttpRequest, HttpResponse, and HttpContext
		// assign resulting context to HttpContext.Current 
		// this was done to better enable ASP.Net Webforms and ASP.NET MVC hybrid websites
        [Test]
        public void TestIPBasedRouting()
        {
            Test.This(controller, url: urlFormat, useHttpContextConstructor: true)
                .When(ctrl => ctrl.Index())
                .ThenExpectViewResultWhereNameIsActionNameOrEmpty();
		}

		// HttpRequest ServerVariable ip mocking
        [Test]
        public void TestIPBasedRouting()
        {
	            Test.This(controller, url: urlFormat, userHostAddress: "127.0.0.1")
                .When(ctrl => ctrl.Index())
                .ThenExpectViewResultWhereNameIsActionNameOrEmpty();
		}

        [Test]
        public void AGetToLogOnReturnsCorrectView()
        {
            Test.This(controller)
                .When(ctrl => ctrl.LogOn())
                .ExpectVerb(HttpVerbs.Get)
                .ExpectViewResultWhereNameIsActionNameOrEmpty();
        }

        [Test]
        public void APostToLogOnWillRedirectToReturnUrl()
        {
            var model = new LogOnModel();

            Test.This(controller)
                .GivenDependenciesAreSetupAs(() => validateUserReturns(true))
                .GivenRequest(request =>
                              request.Setup(r => r.Url).Returns(new Uri("http://xyz.com")))
                .When(ctrl => ctrl.LogOn(model, returnUrl))
                .ExpectRedirectResult(red => red.Url.Equals(returnUrl));
        }

        [Test]
        public void APostToLogOnWillRedirectIfNoReturnUrl()
        {
            var model = new LogOnModel();

            Test.This(controller)
                .GivenDependenciesAreSetupAs(() => validateUserReturns(true))
                .When(ctrl => ctrl.LogOn(model, null))
                .ExpectRedirectToRouteResult(red => red.ActionIs("Index"));
        }

        [Test]
        public void APostToLogOnWillRedirectIfModelIsInvalid()
        {
            var model = new LogOnModel();

            Test.This(controller)
                .Given(ctrl => ctrl.ModelState.AddModelError("somekey", "some error"))
                .When(ctrl => ctrl.LogOn(model, null))
                .ExpectRedirectToRouteResult(red => red.ActionIs("LogOn"))
                .VerifyDependencies(() => shouldSignIn(false));
        }

        [Test]
        public void VerifyEmailWillSetModelSuccessToFalseIfCannotVerifyEmail()
        {
            Test.This(controller)
                .GivenDependenciesAreSetupAs(() =>
                    membershipServiceMock.Setup(m => m.CanVerfiyEmail(It.IsAny<Guid>())).Returns(false))
                .When(ctrl => ctrl.VerifyEmail(Guid.NewGuid()))
                .ExpectViewResultWhereNameIsActionNameOrEmpty()
                .ExpectModel<EmailVerifiedModel>(model => model.WasSuccessful == false);
        }

        [Test]
        public void APostToLogOnWillSendRecordLoginSessionCommand()
        {
            var model = new LogOnModel();

            Test.This(controller)
                .GivenDependenciesAreSetupAs(() => validateUserReturns(true))
                .When(ctrl => ctrl.LogOn(model, null))
                .VerifyDependencies(() =>
                    busMock.Verify(bus => bus.Send(It.IsAny<RecordLoginSession>())));
        }