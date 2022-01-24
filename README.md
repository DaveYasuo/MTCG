# MTCG
## Getting Started (Installation)
### Requirements
- two approaches:
  - installed docker and pulled official [postgres image](https://hub.docker.com/_/postgres) or
  - installed postgres on local machine
### Setup
- clone [this](https://github.com/DaveYasuo/MTCG.git) project and opens up in Visual Studio

- start docker container run following command in terminal:   
  - `docker run --name swe1db -e POSTGRES_USER=swe1user -e POSTGRES_PASSWORD=swe1pw -e POSTGRES_DB=swe1db -p 5432:5432 postgres`
  - to change `--name` there is one line code to be changed in MTCG > Docker > PgEnv.cs `const string containerName = "swe1db";`
  - variables of `POSTGRES_USER` and `POSTGRES_PASSWORD` can be customized
  - `POSTGRES_DB` variable is optional. 
  - port can also be customized.
  - (second approach not tested start docker container using the `docker-compose.yml`, run `docker-compose up` in terminal where the file is located)
- default server port is listening on 10001 (can be changed in MTCG > Program.cs)
- there are two ways to connect to the database
  - entering connection string manually in MTCG > Handler > DataHandler.cs: `using var postgres = new Postgres("host.docker.internal", "5432", "swe1user", "swe1pw", "swe1db", Log, true);` 
  - reading connection variables with the help of the HelperClass PgEnv.cs from the docker environment variables (nothing need to be done, except using other container name `--name` see above)
- now run MTCG in Debug/Release Mode in VS

## Design / Lessons Learned
- To prevent complex class dependencies the concept of Inversion of Control is used. The `DependencyService` can register instances and resolves them automatically.
- The approach was to seperate the TCP-Server from MTCG-Game so that the server can be used for other projects. In order to use the server, there are three classes that must be implemented.
  1. Implement `ILogger` interface and register it to the `DependencyService` (used for outputting  info, exception etc.)
  2. Implement `ISecurity` interface and register it to the `DependencyService` (used for securing paths, authorization token, password generator ect.)
  3. Implement `IRequestHandler` and register it to the `DependencyService` (used for mapping paths to the corresponding functions)
- Also used Factory and Singleton Design pattern.
### Class Diagramm
– link to git <img width="1544" alt="MtcgClassDiagram" src="https://user-images.githubusercontent.com/80542481/150783275-082aa0e9-3218-4acd-82a2-8f47f3d4351f.png">
### Type Dependencies Diagrams
#### ServerModule
##### Business Logic
![Type Dependencies Diagram ServerModule Business logic](https://user-images.githubusercontent.com/80542481/150783363-739fc7b3-96a2-4f87-914f-bc15c2f73d57.png)
##### Organic
![Type Dependencies Diagram for ServerModule Organic](https://user-images.githubusercontent.com/80542481/150783368-b70068ce-e24c-44db-9717-5a0b1706538d.png)
#### MTCG
##### Business Logic
![Type Dependencies Diagram MTCG business logic](https://user-images.githubusercontent.com/80542481/150783419-9f66649e-1460-4696-aaf2-67b1dda7fb0c.png)
##### Organic
![Type Dependencies Diagram MTCG organic](https://user-images.githubusercontent.com/80542481/150783427-871ec87c-f3c6-45e4-b204-d2cf949324b4.png)
## Failures and selected solutions
First problem was when testing response in Postman that all headers were in response payload. To solve this problem was to write to the client stream directly instead of storing the headers in a Stringbuilder. Another problem was that `RequestHandler` (mtcg) is dependent on `Authorization` (server), which is dependent on `Security` (mtcg). Therefore dependency injections were solved with the `DependencyService`.
## Tests Coverage
### Unit tests
Unit tests covered the battle functionality, the cards creation and all sort of damage calculation. Those tests were chosen since the Card-Game is the main functionality of this project.
### Integration tests with Postman
Curl scripts test the integration of the database and the tcp-server. Provided by the [exercise](https://github.com/DaveYasuo/MTCG/blob/9acda0fddf887b4b4bf444f086dc2c1b0418ebc1/MonsterTradingCards.exercise.curl.bat).
## Collection Types
- mostly used is the `Dictionary` for mapping (path,functions), headers, storing dependencies etc.
- used `ConcurrentDictionary` for storing user sessions and tasks
- used `ConcurrentQueue` for queuing player to the game queue
- used `List` for cards, routes, packages etc.

## Time spent
Overall for research, implementing and testing: 222 hours

## Game Logic
[See exercice](https://github.com/DaveYasuo/MTCG/blob/9acda0fddf887b4b4bf444f086dc2c1b0418ebc1/MonsterTradingCards_exercise.pdf)





