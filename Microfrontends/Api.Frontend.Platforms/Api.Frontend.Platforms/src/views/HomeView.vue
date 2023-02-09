<template>
  <div>
    <b-row>
      <platforms-data-table></platforms-data-table>
    </b-row>
  </div>
</template>

<script>
import PlatformsDataTable from "@/components/PlatformsDataTable.vue";

export default {
  name: "Home-View",
  components: {
    PlatformsDataTable,
  },
};
</script>
