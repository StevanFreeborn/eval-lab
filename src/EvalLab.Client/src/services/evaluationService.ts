import { InjectionKey } from 'vue';
import { Page, result, Result } from './shared';

type NewEvaluation = {
  name: string;
  description: string;
};

export type Evaluation = NewEvaluation & {
  id: string;
  createdDate: Date;
  updatedDate: Date;
};

type EvaluationsService = {
  createEvaluation(newEvaluation: NewEvaluation): Promise<Result<Evaluation>>;
  getEvaluations(): Promise<Result<Page<Evaluation>>>;
  deleteEvaluation(id: string): Promise<Result<true>>;
};

type EvaluationsServiceKeyType = InjectionKey<EvaluationsService>;

export const EvaluationsServiceKey: EvaluationsServiceKeyType = Symbol('EvaluationsService');

const BASE_URL = '/api/evaluations';

export const evaluationsService: EvaluationsService = Object.freeze({
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

    return result.success(createEvaluation(data));
  },
  async getEvaluations() {
    const response = await fetch(BASE_URL);

    if (response.ok === false) {
      return result.failure(new Error('Failed to get evaluations'));
    }

    const data = await response.json();

    return result.success({
      ...data,
      items: data.items.map(createEvaluation),
    });
  },
  async deleteEvaluation(id) {
    const response = await fetch(`${BASE_URL}/${id}`, { method: 'DELETE' });

    if (response.ok === false) {
      return result.failure(new Error('Failed to delete evaluation'));
    }

    return result.success(true);
  },
});

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
