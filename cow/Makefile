.PHONY: build run run-hello run-fib test clean

file ?= hello.cow

build:
	dotnet build CowInterpreter.sln

run: build
	dotnet run --project CowInterpreter/CowInterpreter.csproj -- $(file)

run-hello:
	$(MAKE) run file=hello.cow

run-fib:
	$(MAKE) run file=fib.cow

test:
	dotnet test CowInterpreter.sln

coverage:
	dotnet test CowInterpreter.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage/
	reportgenerator -reports:./CowInterpreter.Tests/coverage/coverage.opencover.xml -targetdir:./coverage-report -reporttypes:Html
	@echo "HTML report generated in ./coverage-report/index.html"

clean:
	dotnet clean CowInterpreter.sln
	rm -rf CowInterpreter/bin
	rm -rf CowInterpreter/obj
	rm -rf CowInterpreter.Tests/bin
	rm -rf CowInterpreter.Tests/obj
