.PHONY: build run test coverage clean

build:
	dotnet build

run:
	dotnet run --project src/BestProgram.App/best_program.csproj

test:
	dotnet test

coverage:
	dotnet test BestProgram.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage/
	dotnet tool run reportgenerator -reports:./tests/BestProgram.Tests/coverage/coverage.opencover.xml -targetdir:./coverage-report -reporttypes:Html
	@echo "HTML report: ./coverage-report/index.html"

clean:
	rm -rf coverage-report **/TestResults **/coverage
