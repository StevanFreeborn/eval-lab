import { InjectionKey } from 'vue';
import { Evaluation } from './evaluationService';
import { createGenericService, Entity, GenericService } from './shared';

export type NewEvaluationRun = {
  expectedProportion: number;
  confidenceLevel: number;
  marginOfError: number;
  evaluation: Evaluation;
};

export type EvaluationRun = Omit<NewEvaluationRun, 'evaluation'> &
  Entity & {
    evaluationId: string;
    sampleSize: number;
    status: string;
  };

type EvaluationRunsService = GenericService<NewEvaluationRun, EvaluationRun>;

type EvaluationRunsServiceKeyType = InjectionKey<EvaluationRunsService>;

export const EvaluationRunsServiceKey: EvaluationRunsServiceKeyType =
  Symbol('EvaluationRunsService');

const BASE_URL = '/api/evaluations/runs';

export const evaluationRunService: EvaluationRunsService = Object.freeze(
  createGenericService(BASE_URL, createEvaluationRun),
);

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createEvaluationRun(data: any): EvaluationRun {
  return {
    id: data.id,
    name: data.name,
    expectedProportion: data.expectedProportion,
    confidenceLevel: data.confidenceLevel,
    marginOfError: data.marginOfError,
    evaluationId: data.evaluationId,
    sampleSize: data.sampleSize,
    status: data.status,
    createdDate: new Date(data.createdDate),
    updatedDate: new Date(data.updatedDate),
  };
}
