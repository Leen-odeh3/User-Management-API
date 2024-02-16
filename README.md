## Identity (ASP.NET Core)
<br>


| Method Name     | Method Type | Functionality                                                                                      | Response Data Format                                                                                          |
|-----------------|-------------|---------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------|
| Register        | POST        | Registers a new user. Checks if the user already exists, then creates a new user and sends a confirmation email.                                             | JSON: { "Status": "Success/Error", "Message": "User created & Email Sent to {email} SuccessFully / Error message" } |
| GetAllUsers     | GET         | Retrieves all users.                                                                              | JSON: Array of User objects                                                                                    |
| DeleteUser      | DELETE      | Deletes a user by ID.                                                                             | JSON: { "Status": "Success/Error", "Message": "User deleted successfully / Failed to delete user" }             |
| UpdateUser      | PUT         | Updates user details based on the provided user ID and updated user DTO.                           | JSON: { "Status": "Success/Error", "Message": "User updated successfully / Failed to update user" }             |
| ConfirmEmail    | GET         | Confirms the user's email using the provided token and email.                                      | JSON: { "Status": "Success/Error", "Message": "Email Verified Successfully / This User Does not exist!" }       |
| Login           | POST        | Handles user login. Validates credentials, generates a JWT token, and sends an OTP if two-factor authentication is enabled.                              | JSON: { "token": "JWT token string", "expiration": "Expiration date/time" }                                    |
| LoginWithOTP    | POST        | Handles user login using OTP. Validates the OTP, generates a JWT token, and returns it in the response.                                                      | JSON: { "token": "JWT token string", "expiration": "Expiration date/time" }                                    |
| ForgotPassword  | POST        | Sends a password reset link to the user's email address.                                           | JSON: { "Status": "Success/Error", "Message": "Reset Forgotten Password Request has been sent to email: {email}" }|
| ResetPassword   | POST        | Resets the user's password using the provided token and new password.                              | JSON: { "Status": "Success/Error", "Message": "Password has successfully been reset for account with email: {email}" }|


<br>

## DB & Result:
<img width="208" alt="db" src="https://github.com/Leen-odeh3/User-Management-API/assets/123558998/58c04b8a-0e7d-430f-a113-7c47ebbc2bf4">
<img width="713" alt="register" src="https://github.com/Leen-odeh3/User-Management-API/assets/123558998/0d2e60a6-bcda-49ea-a490-488c794d9292">
<img width="707" alt="response" src="https://github.com/Leen-odeh3/User-Management-API/assets/123558998/c8cf8729-9d3d-46d0-b611-798167bf44e8">
 <img width="533" alt="forgot-pass" src="https://github.com/Leen-odeh3/User-Management-API/assets/123558998/4e15384f-08fd-4a4c-962c-020ba9cb64d9">
<img width="707" alt="forgot-link" src="https://github.com/Leen-odeh3/User-Management-API/assets/123558998/aeed4170-0468-4a9c-a7c9-45eb0d642e0c"> <img width="842" alt="getToken" src="https://github.com/Leen-odeh3/User-Management-API/assets/123558998/6724c8fa-2561-4436-bc49-41c17296c024">
<img width="316" alt="2f" src="https://github.com/Leen-odeh3/User-Management-API/assets/123558998/ddae243c-8433-4867-ae5a-eab51562f8e4">

