//#define NC_AUTH_GOOGLE
//#define NC_AUTH_FACEBOOK
//#define NC_AUTH_MICROSOFT
//#define NC_AUTH_TWITTER

using System;

using Foundation;
using UIKit;

using Producer.Auth;

#if NC_AUTH_GOOGLE
#endif
#if NC_AUTH_FACEBOOK
#endif
#if NC_AUTH_MICROSOFT
#endif
#if NC_AUTH_TWITTER
#endif


#if NC_AUTH_GOOGLE
using Google.SignIn;
#endif
#if NC_AUTH_FACEBOOK
using Facebook.CoreKit;
using Facebook.LoginKit;
#endif
#if NC_AUTH_MICROSOFT
#endif
#if NC_AUTH_TWITTER
#endif

namespace Producer.iOS
{
	public partial class LoginVc : UIViewController
#if NC_AUTH_GOOGLE
	, ISignInDelegate, ISignInUIDelegate
#endif
	{
		SignInButton _authButtonGoogle, _authButtonMicrosoft, _authButtonTwitter, _authButtonFacebook;

		SignInButton AuthButtonGoogle => _authButtonGoogle ?? (_authButtonGoogle = new SignInButton { Tag = 0 });
		SignInButton AuthButtonFacebook => _authButtonFacebook ?? (_authButtonFacebook = new SignInButton { Tag = 1 });
		SignInButton AuthButtonMicrosoft => _authButtonMicrosoft ?? (_authButtonMicrosoft = new SignInButton { Tag = 2 });
		SignInButton AuthButtonTwitter => _authButtonTwitter ?? (_authButtonTwitter = new SignInButton { Tag = 3 });

		public LoginVc (IntPtr handle) : base (handle) { }


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			initSignInButtons ();
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			connectSignInButtonHandlers ();
		}


		public override void ViewDidDisappear (bool animated)
		{
			disconnectSignInButtonHandlers ();

			base.ViewDidDisappear (animated);
		}


		partial void cancelClicked (NSObject sender) => DismissViewController (true, null);


		void initSignInButtons ()
		{
#if NC_AUTH_GOOGLE

			buttonStackView.AddArrangedSubview (AuthButtonGoogle);

			SignIn.SharedInstance.UIDelegate = this;
			SignIn.SharedInstance.Delegate = this;

			// Uncomment to automatically sign in the user.
			SignIn.SharedInstance.SignInUserSilently ();

			// Uncomment to automatically sign out the user.
			// SignIn.SharedInstance.SignOutUser ();
#endif
#if NC_AUTH_FACEBOOK

			stackView.AddArrangedSubview (AuthButtonFacebook);

			if (AccessToken.CurrentAccessToken != null)
			{
				// User is logged in, do work such as go to next view controller.
				Log.Debug ($"Facebook Current Access Token: {AccessToken.CurrentAccessToken}");
			}
			else
			{
				Log.Debug ($"Facebook Current Access Token: null");
			}

#endif
#if NC_AUTH_MICROSOFT
			stackView.AddArrangedSubview (AuthButtonMicrosoft);
#endif
#if NC_AUTH_TWITTER
			stackView.AddArrangedSubview (AuthButtonTwitter);
#endif
			buttonStackViewHeightConstraint.Constant = buttonStackView.ArrangedSubviews.Length * 52;
		}



		#region SignInButton Click Handlers

#if NC_AUTH_GOOGLE

		void handleAuthButtonGoogleClicked (object s, EventArgs e)
		{
			SignIn.SharedInstance.SignInUser ();
		}

#endif

#if NC_AUTH_FACEBOOK

		void handleAuthButtonFacebookClicked (object s, EventArgs e)
		{
			var readPermissions = new string [] { @"public_profile", @"email"/*, @"user_friends"*/};

			var loginManager = new LoginManager ();

			loginManager.LogInWithReadPermissions (readPermissions, this, (LoginManagerLoginResult result, NSError error) =>
			{
				if (error != null)
				{
					Log.Error ($"Facebook Login Failed: Code: {error.Code}, Description: {error.LocalizedDescription}");
				}
				else if (result.IsCancelled)
				{
					Log.Debug ("Facebook Login Failed: Cancelled");
				}
				else
				{
					Log.Debug ($"Facebook Login Success: Token: {result.Token}");

					DismissViewController (true, null);
				}
			});
		}

#endif

#if NC_AUTH_MICROSOFT

		void handleAuthButtonMicrosoftClicked (object s, EventArgs e)
		{
			showNotImplementedAlert ("Microsoft");
		}

#endif

#if NC_AUTH_TWITTER

		void handleAuthButtonTwitterClicked (object s, EventArgs e)
		{
			showNotImplementedAlert ("Twitter");
		}

#endif

		void showNotImplementedAlert (string providerName)
		{
			var alert = UIAlertController.Create ("Bummer", $"Looks like this lazy developer hasn't implemented {providerName} auth yet.", UIAlertControllerStyle.Alert);

			alert.AddAction (UIAlertAction.Create ("Complain", UIAlertActionStyle.Destructive, handleComplainAction));
			alert.AddAction (UIAlertAction.Create ("Whatever", UIAlertActionStyle.Default, handleComplainAction));

			PresentViewController (alert, true, null);

			void handleComplainAction (UIAlertAction action)
			{
				if (action.Title == "Complain")
				{
					var issueUrl = @"https://github.com/technicalpoets/producer/issues/new";

					UIApplication.SharedApplication.OpenUrl (new NSUrl (issueUrl));

					// DismissViewController (true, null;
				}
				else if (action.Title == "Whatever")
				{
					//DismissViewController (true, null);
				}
			}
		}


		void connectSignInButtonHandlers ()
		{
#if NC_AUTH_GOOGLE
			AuthButtonGoogle.TouchUpInside += handleAuthButtonGoogleClicked;
#endif
#if NC_AUTH_FACEBOOK
			AuthButtonFacebook.TouchUpInside += handleAuthButtonFacebookClicked;
#endif
#if NC_AUTH_MICROSOFT
			AuthButtonMicrosoft.TouchUpInside += handleAuthButtonMicrosoftClicked;
#endif
#if NC_AUTH_TWITTER
			AuthButtonTwitter.TouchUpInside += handleAuthButtonTwitterClicked;
#endif
		}


		void disconnectSignInButtonHandlers ()
		{
#if NC_AUTH_GOOGLE
			AuthButtonGoogle.TouchUpInside -= handleAuthButtonGoogleClicked;
#endif
#if NC_AUTH_FACEBOOK
			AuthButtonFacebook.TouchUpInside -= handleAuthButtonFacebookClicked;
#endif
#if NC_AUTH_MICROSOFT
			AuthButtonMicrosoft.TouchUpInside -= handleAuthButtonMicrosoftClicked;
#endif
#if NC_AUTH_TWITTER
			AuthButtonTwitter.TouchUpInside -= handleAuthButtonTwitterClicked;
#endif
		}

		#endregion


#if NC_AUTH_GOOGLE

		#region ISignInDelegate

		public void DidSignIn (SignIn signIn, GoogleUser user, NSError error)
		{
			if (error == null && user != null)
			{
				ClientAuthManager.Shared.SetClientAuthDetails (user.GetAuthDetails ());

				DismissViewController (true, null);
			}
			else
			{
				Log.Error (error?.LocalizedDescription);
			}
		}


		[Export ("signIn:didDisconnectWithUser:withError:")]
		public void DidDisconnect (SignIn signIn, GoogleUser user, NSError error)
		{
			Log.Debug ("Google User DidDisconnect");

			// Perform any operations when the user disconnects from app here.
		}

		#endregion

#endif

	}
}
