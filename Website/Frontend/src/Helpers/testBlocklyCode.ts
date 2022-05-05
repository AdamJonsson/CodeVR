import Task from '../Types/Task';

function testBlocklyCode(code: string, task: Task): [boolean, string, string, string] {
  var allTestsPassed = true;
  var inputs: string[] = [];
  var expectedOutput = "";
  var outputFromCurrentlyBlocklyCode: string = "";
  
  try {
    for (const testCase of task.testCases) {
      outputFromCurrentlyBlocklyCode = "";
      inputs = testCase.inputs;
      expectedOutput = testCase.output;

      setTimeout(function(){
        eval(
          code +
          `outputFromCurrentlyBlocklyCode = ${task.functionName}(${testCase.inputs.join(",")});`
        );
      }, 250);

      try {
        outputFromCurrentlyBlocklyCode = outputFromCurrentlyBlocklyCode.toString();
      } catch (error) {}

      if (outputFromCurrentlyBlocklyCode !== testCase.output)
      {
        allTestsPassed = false;
        break;
      }
    }
  } catch (error: any) {
    allTestsPassed = false;
    outputFromCurrentlyBlocklyCode = error.message as string;
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

  return [allTestsPassed, readableInputs, expectedOutput, outputFromCurrentlyBlocklyCode];
}

export default testBlocklyCode;