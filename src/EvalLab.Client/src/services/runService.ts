import { InjectionKey } from 'vue';
import { createGenericService, Entity, GenericService } from './shared';

export type NewRun = {
  pipelineId: string;
  input: string;
};

export type Run = NewRun &
  Entity & {
    output: string;
  };

type RunsService = GenericService<NewRun, Run>;

type RunsServiceKeyType = InjectionKey<RunsService>;

export const RunsServiceKey: RunsServiceKeyType = Symbol('RunsService');

const BASE_URL = '/api/runs';

export const runsService: RunsService = Object.freeze(createGenericService(BASE_URL, createRun));

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createRun(data: any): Run {
  return {
    pipelineId: data.pipelineId,
    id: data.id,
    name: data.name,
    input: data.input,
    output: data.output,
    createdDate: new Date(data.createdDate),
    updatedDate: new Date(data.updatedDate),
  };
}
