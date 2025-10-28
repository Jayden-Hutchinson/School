# Elixir Assignment 1

Implement a system in Elixir that allows users to send messages to one another. The user connects to the system via TCP and issues commands.

## Commands

_Case Insensitive_

_Terminated by the end-of-line character sequence_

_Any other commands used are invalid_

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

- List of nick names is separated by commas
- If followed by fewer than 2 words, invalid
- Only registered users can send and receive messages

`/GRP <groupname> <users>`
: Set a group name to a list of one or more users

- groupname must start with `#`
- groupname includes underscore and alphanumeric
- groupname max length of 11 characters (including `#`)

## Modules

### Client

- Elixir or Java
- "dumb" client; Does not perform command validation

### Server

`Globally-Registered Supervised Server`
`GenServer`

- Message Dispatching
- Nickname Handling
- ETS table state
- Does not communicate with external clients

### ProxyServer

`Elixir Server`

- Accepts external clients that connect via TCP
- Not registered
- Can run different instances of this on different nodes
- Creates an elixir process for each TCP connection
- Command validation

### Proxy

- Command validation

Program terminates when the user enters the end-of-file key at the beginning of an input line

To facilitate testingthe system must print debugging information

## Submission

**Deadline:** `DD/MM/YY`

**File:** `assignmet1.zip` _containing the project folder 'chat' excluding \_build_

_You will need to set up and demonstrate your chat system as well as explain, and possibly answer questions about your code_
