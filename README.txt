Firefly Backend Test

The intention of this test is to provide an API that allows a consumer to create a basic class structure containing students and teachers.

1. Complete the API controllers to provide creation and retrieval for Classes, Students and Teachers.
2. Complete the methods AssignClass in the TeachersController and EnrollStudents in the ClassesController.
3. Add data validation for the StudentDto, TeacherDto and ClassDto so that the following holds:
	- A Class must have a name which is a maximum of 20 characters
	- A Class must have a capacity which is a minimum of 5 and a maximum of 30
	- A Teacher must have a name which is a maximum of 50 characters
	- A Student must have a name which is a maximum of 50 characters
4. Add business rule validation so that:
	- A student cannot be enrolled in a class that is over its capacity
	- A teacher cannot be assigned to more that 5 classes
	- Only one teacher may be assigned per class
5. Provide unit tests for the validations detailed in step 3


Improvements:

Better logging of events and http request/responses
Move validation fro EnrollStudentAsync and AssignClassAsync into separate injected validation service
Perhaps add mediator pipeline for logging and mapping????
Fix stupid issue with database test assertions!!