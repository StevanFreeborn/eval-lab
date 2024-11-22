import { InjectionKey } from 'vue';
import { Page, result, Result } from './shared';

type NewEvaluation = {
  name: string;
  description: string;
};

export type Evaluation = NewEvaluation & {
  id: number;
  createdDate: Date;
  updatedDate: Date;
};

type EvaluationsService = {
  getEvaluations(): Promise<Result<Page<Evaluation>>>;
  createEvaluation(newEvaluation: NewEvaluation): Promise<Result<Evaluation>>;
};

type EvaluationsServiceKeyType = InjectionKey<EvaluationsService>;

export const EvaluationsServiceKey: EvaluationsServiceKeyType = Symbol('EvaluationsService');

const BASE_URL = '/api/evaluations';

export const evaluationsService: EvaluationsService = Object.freeze({
  async getEvaluations() {
    const response = await fetch(BASE_URL);

    if (response.ok === false) {
      return result.failure(new Error('Failed to get evaluations'));
    }

    const data = await response.json();

    return result.success(data);
  },
  async createEvaluation(newEvaluation) {
    const response = await fetch(BASE_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newEvaluation),
    });

    if (response.ok === false) {
      return result.failure(new Error('Failed to create evaluation'));
    }

    const data = await response.json();

    return result.success(data);
  },
});
