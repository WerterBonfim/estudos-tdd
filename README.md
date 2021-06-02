My resource for TDD, BDD, Integration testing, automation testing and cargo test

I learned through:
* [desenvolvedor.io][dev.io] | Cource author MVP [Eduardo Pires][eduardo] 


[dev.io]:https://desenvolvedor.io/
[eduardo]:https://github.com/EduardoPires




## How to execute

I'm using Linux to developer this tutorial

With Docker:
```bash
git clone https://github.com/WerterBonfim/estudos-tdd.git

cd estudos-tdd/docker

docker build -t sql-dev .
docker container run --name sql -d -p 1433:1433 sql-dev

```


## [Section 3 - Integrations testing][level01.integrations]

 * 1 - lorem    
 * 2 - lorem
 * 3 - lorem

 [level01.integrations]:03-Teste-de-Integracao/README.md

# Todo


```
Microsoft.EntityFrameworkCore 5.0.6
Microsoft.EntityFrameworkCore.SqlServer 5.0.6
Microsoft.EntityFrameworkCore.Desing 5.0.6
Microsoft.EntityFrameworkCore.Tools 5.0.6
Microsoft.EntityFrameworkCore.SQLite 5.0.6

coverlet.collector 3.0.3
FluentAssertions 5.10.3
Microsoft.NET.Test.Sdk 16.10.0
Moq 4.16.1
Moq.AutoMock 2.3.0
xunit 2.4.1
xunit.runner.visualstudio 2.4.3
```
