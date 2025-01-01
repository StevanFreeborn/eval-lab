import { InjectionKey } from 'vue';
import { createPipelineRun, PipelineRun } from './pipelineRunService';
import {
  createGenericService,
  Entity,
  GenericService,
  makeRequest,
  result,
  Result,
} from './shared';

type NewEvaluation = {
  name: string;
  description: string;
};

type SuccessCriteria =
  | { type: 'null' }
  | {
      type: 'Unstructured Exact Match' | 'Unstructured Partial Match';
      matchValue: string;
    }
  | {
      type: 'JSON Match';
      schema: string;
    };

export type Evaluation = NewEvaluation &
  Entity & {
    targetPipelineId: string;
    successCriteria: SuccessCriteria;
  };

type TestEvaluation = {
  input: string;
  evaluation: Evaluation;
};

export type TestRun = {
  pipelineRun: PipelineRun;
  passed: boolean;
};

type EvaluationsService = GenericService<NewEvaluation, Evaluation> & {
  test: (evaluation: TestEvaluation) => Promise<Result<TestRun>>;
};

type EvaluationsServiceKeyType = InjectionKey<EvaluationsService>;

export const EvaluationsServiceKey: EvaluationsServiceKeyType = Symbol('EvaluationsService');

const BASE_URL = '/api/evaluations';

export const evaluationsService: EvaluationsService = Object.freeze({
  ...createGenericService(BASE_URL, createEvaluation),
  test: async function (evaluationRun) {
    const response = await makeRequest<TestRun>(`${BASE_URL}/${evaluationRun.evaluation.id}/test`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(evaluationRun),
    });

    if (response.failed) {
      return response;
    }

    return result.success(createTestRun(response.value));
  },
});

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createEvaluation(data: any): Evaluation {
  return {
    id: data.id,
    name: data.name,
    description: data.description,
    targetPipelineId: data.targetPipelineId,
    successCriteria: createSuccessCriteria(data.successCriteria),
    createdDate: new Date(data.createdDate),
    updatedDate: new Date(data.updatedDate),
  };
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createSuccessCriteria(data: any): SuccessCriteria {
  if (data.type === 'Unstructured Exact Match' || data.type === 'Unstructured Partial Match') {
    return {
      type: data.type,
      matchValue: data.matchValue,
    };
  }

  if (data.type === 'JSON Match') {
    return {
      type: data.type,
      schema: data.schema,
    };
  }

  return {
    type: 'null',
  };
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createTestRun(data: any): TestRun {
  return {
    pipelineRun: createPipelineRun(data.pipelineRun),
    passed: data.passed,
  };
}
