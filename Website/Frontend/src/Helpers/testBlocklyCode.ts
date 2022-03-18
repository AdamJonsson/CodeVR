import Task from '../Types/Task';

function testBlocklyCode(code: string, task: Task): [boolean, string, string, string] {
  var allTestsPassed = true;
  var inputs: string[] = [];
  var expectedOutput = "";
  var output: string = "";
  
  try {
    for (const testCase of task.testCases) {
      output = "";
      inputs = testCase.inputs;
      expectedOutput = testCase.output;

      eval(
        code +
        `output = ${task.functionName}(${testCase.inputs.join(",")});`
      );

      try {
        output = output.toString();
      } catch (error) {}

      if (output !== testCase.output)
      {
        allTestsPassed = false;
        break;
      }
    }
  } catch (error: any) {
    allTestsPassed = false;
    output = error.message as string;
  }

  var readableInputs = ""
  for (let index = 0; index < inputs.length; index++) {
    const input = inputs[index];
    const variable = task.variables[index];
    readableInputs += variable + ": " + input;
    if (index !== inputs.length - 1)
    {
      readableInputs += ", ";
    }
  }

  return [allTestsPassed, readableInputs, expectedOutput, output];
}

export default testBlocklyCode;