## FlashCards application
Application built using .NET 6 with code-first approach and MVC architecture. Client side of the application was built using Razor syntax with Bootstrap and JavaScript. Database using Microsoft SQL Server, database querying using Entity Framework. Application authorization utilizes .NET Identity.

Application is currently hosted using Azure. To access it, click the link below: 

(Server might need a few seconds to get back up)

[Hosted application](https://psflashcards.azurewebsites.net/)

Users are welcome to create their own accounts, or use one of already existing ones:

**admin**: 

login: admin@admin.com 

password: !Q1w2e3r4

**user**: 

login: user@user.com

password: !Q1w2e3r4

**user2**: 

login: user2@user.com

password: !Q1w2e3r4

For accounts, sets were created, reports were submitted, notifications received - in order to fully display the application ecosystem.

### Description

Application meant to be a community tool for creating flashcard sets and accessing them to learn terms by repetition or quizzes. 

Users can create their own sets of terms or can browse sets, using filters, grouping them by categories or more specific - subjects. Sets can be made public to the community, allowing others to access said sets and copy them in order to make their own changes.

Users can "learn" the set by going through the terms like in the real-world scenario by "flipping" cards and comparing their guesses to the correct answer. To make things more challenging, sets can be shuffled, to appear in different order. To test their knowledge, quizzes can be accessed. Quizzes are meant for sets with at least 4 cards and provide user with a term and four different answers that the user has to choose from.

To maintain a high level of public sets, users can report sets that in their opinion do not meet the standards.

Reports can be evaluated by an administrator-user, who can take different actions on what to do with reported set:
- he can do nothing and discard the report, 
- he can hide the set from public access, and notify the owner of reported set,
- he can delete the set completely, and notify the owner of reported set,
- he can take no action on the set in question but can send a message to user to inform him about a problem with his set.

To keep users informed about their reports and reports on their sets, a Notification service was implemented.

> Some parts of UI were inspired by website Quizlet.com