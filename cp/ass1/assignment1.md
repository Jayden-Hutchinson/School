# Elixir Assignment 1

Implement a system in Elixir that allows users to send messages to one another. The user connects to the system via TCP and issues commands.

## Commands
*Case Insensitive*

*Terminated by the end-of-line character sequence*

*Any other commands used are invalid*

`/NCK <nickname>`
: Set a nickname
- Takes one argument; any extra words are ignored
- No argument is invalid 
- Provided nickname must not be in use
- Sends a response message of success/failure
- User must set a nickname before they can send or receive messages
- Can change an existing username
- A user with a nickname is a `Registered User`
- Provided `<nickname>` must start with an alphabet followed by alphanumeric or uderscore characters. Max Length of 10 characters

`/LST`
: Get a list of nicknames currently in use

`/MSG <recipients> <message>`
: Send a message to a specific user or a list of users.

`/GRP <groupname> <users>`
: Set a group name to a list of one or more users

## Submission
**Deadline:** `DD/MM/YY`

**File:** `assignmet1.zip` *containing the project folder 'chat' excluding _build*

*You will need to set up and demonstrate your chat system as well as explain, and possibly answer questions about your code*