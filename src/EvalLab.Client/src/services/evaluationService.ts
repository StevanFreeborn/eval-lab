import { InjectionKey } from 'vue';
import { Run } from './runService';
import { createGenericService, Entity, GenericService, makeRequest, Result } from './shared';

type NewEvaluation = {
  name: string;
  description: string;
};

type SuccessCriteria =
  | { type: 'null' }
  | {
      type: 'Unstructured Exact Match';
      matchValue: string;
    };

export type Evaluation = NewEvaluation &
  Entity & {
    targetPipelineId: string;
    input: string;
    successCriteria: SuccessCriteria;
  };

type TestResult = {
  run: Run;
  passed: boolean;
};

type EvaluationsService = GenericService<NewEvaluation, Evaluation> & {
  test: (evaluation: Evaluation) => Promise<Result<TestResult>>;
};

type EvaluationsServiceKeyType = InjectionKey<EvaluationsService>;

export const EvaluationsServiceKey: EvaluationsServiceKeyType = Symbol('EvaluationsService');

const BASE_URL = '/api/evaluations';

export const evaluationsService: EvaluationsService = Object.freeze({
  ...createGenericService(BASE_URL, createEvaluation),
  test: async function (evaluation) {
    return await makeRequest<TestResult>(`${BASE_URL}/${evaluation.id}/test`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(evaluation),
    });
  },
});

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createEvaluation(data: any): Evaluation {
  return {
    id: data.id,
    name: data.name,
    description: data.description,
    targetPipelineId: data.targetPipelineId,
    input: data.input,
    successCriteria: createSuccessCriteria(data.successCriteria),
    createdDate: new Date(data.createdDate),
    updatedDate: new Date(data.updatedDate),
  };
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createSuccessCriteria(data: any): SuccessCriteria {
  if (data.type === 'Unstructured Exact Match') {
    return {
      type: data.type,
      matchValue: data.matchValue,
    };
  }

  return {
    type: 'null',
  };
}
