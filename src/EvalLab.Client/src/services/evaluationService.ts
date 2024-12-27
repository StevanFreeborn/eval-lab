import { InjectionKey } from 'vue';
import { createGenericService, Entity, GenericService } from './shared';

type NewEvaluation = {
  name: string;
  description: string;
};

export type Evaluation = NewEvaluation & Entity;

type EvaluationsService = GenericService<NewEvaluation, Evaluation, Evaluation>;

type EvaluationsServiceKeyType = InjectionKey<EvaluationsService>;

export const EvaluationsServiceKey: EvaluationsServiceKeyType = Symbol('EvaluationsService');

const BASE_URL = '/api/evaluations';

export const evaluationsService: EvaluationsService = Object.freeze(
  createGenericService(BASE_URL, createEvaluation, createEvaluation),
);

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createEvaluation(data: any): Evaluation {
  return {
    id: data.id,
    name: data.name,
    description: data.description,
    createdDate: new Date(data.createdDate),
    updatedDate: new Date(data.updatedDate),
  };
}
