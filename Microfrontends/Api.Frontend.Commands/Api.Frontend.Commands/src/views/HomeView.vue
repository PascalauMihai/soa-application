<template>
  <div>
    <b-row>
      <platforms-commands-data-table></platforms-commands-data-table>
    </b-row>
  </div>
</template>

<script>
import PlatformsCommandsDataTable from "@/components/PlatformsCommandsDataTable.vue";

export default {
  name: "Home-View",
  components: {
    PlatformsCommandsDataTable,
  },
};
</script>
